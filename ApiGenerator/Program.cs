using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace ApiGenerator
{
    class Program
    {
        // ============ Property Overrides ============
        // نام Property توی DataContext برای جدول‌هایی که با نام جدول فرق دارن
        private static readonly Dictionary<string, string> PropertyOverrides =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "X_DigitalFileAccess", "X_DigitalFileAccesses" },
            { "X_help", "X_helps" },
            { "X_OrderShipping", "X_OrderShippings" },
            { "X_OrderStatusHistory", "X_OrderStatusHistories" }
        };

        // ============ Entity Name Overrides ============
        // نام Entity برای جدول‌هایی که مفرد کردن روشون جواب نمی‌ده
        private static readonly Dictionary<string, string> EntityOverrides =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "X_help", "Help" },
            { "X_DigitalFileAccess", "DigitalFileAccess" },
            { "X_OrderShipping", "OrderShipping" },
            { "X_OrderStatusHistory", "OrderStatusHistory" }
        };

        static void Main()
        {
            Console.WriteLine("=== API Code Generator ===");
            Console.WriteLine();

            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            if (!File.Exists(configPath))
            {
                Console.WriteLine("appsettings.json not found!");
                Console.ReadKey();
                return;
            }

            var config = JObject.Parse(File.ReadAllText(configPath));
            string connStr = config["ConnectionString"].ToString();
            string outputPath = config["OutputPath"].ToString();
            string ns = config["Namespace"].ToString();

            Console.WriteLine("Output: " + outputPath);
            Console.WriteLine("Namespace: " + ns);
            Console.WriteLine();

            var tables = ReadSchema(connStr);
            Console.WriteLine("Tables found: " + tables.Count);
            Console.WriteLine();

            var viewModelsDir = Path.Combine(outputPath, "ViewModels", "Generated");
            var controllersDir = Path.Combine(outputPath, "Controllers", "Generated");
            var baseDir = Path.Combine(outputPath, "Controllers", "Base");

            Directory.CreateDirectory(viewModelsDir);
            Directory.CreateDirectory(controllersDir);
            Directory.CreateDirectory(baseDir);

            int created = 0, updated = 0, skipped = 0;

            var baseResult = WriteIfChanged(
                Path.Combine(baseDir, "BaseApiController.cs"),
                GenerateBaseController(ns));
            Report(baseResult, "BaseApiController.cs", ref created, ref updated, ref skipped);

            foreach (var t in tables)
            {
                var vmPath = Path.Combine(viewModelsDir, t.EntityName + "ViewModel.cs");
                var vmResult = WriteIfChanged(vmPath, GenerateViewModel(ns, t));
                Report(vmResult, "ViewModels/" + t.EntityName + "ViewModel.cs", ref created, ref updated, ref skipped);

                var ctrlPath = Path.Combine(controllersDir, t.EntityName + "Controller.cs");
                var ctrlResult = WriteIfChanged(ctrlPath, GenerateController(ns, t));
                Report(ctrlResult, "Controllers/" + t.EntityName + "Controller.cs", ref created, ref updated, ref skipped);
            }

            Console.WriteLine();
            Console.WriteLine("=== Done ===");
            Console.WriteLine("Created: " + created);
            Console.WriteLine("Updated: " + updated);
            Console.WriteLine("Skipped: " + skipped);
            Console.WriteLine();
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        static void Report(string result, string name, ref int c, ref int u, ref int s)
        {
            if (result == "created") { Console.WriteLine("  + " + name); c++; }
            else if (result == "updated") { Console.WriteLine("  ~ " + name); u++; }
            else s++;
        }

        static string WriteIfChanged(string path, string content)
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            if (!File.Exists(path))
            {
                File.WriteAllText(path, content, Encoding.UTF8);
                return "created";
            }

            var existing = File.ReadAllText(path, Encoding.UTF8);
            if (existing == content) return "skipped";

            File.WriteAllText(path, content, Encoding.UTF8);
            return "updated";
        }

        static List<TableInfo> ReadSchema(string connStr)
        {
            var tables = new List<TableInfo>();

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();

                var tablesCmd = new SqlCommand(
                    "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES " +
                    "WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME LIKE 'X[_]%' " +
                    "ORDER BY TABLE_NAME",
                    conn);

                var tableNames = new List<string>();
                using (var r = tablesCmd.ExecuteReader())
                {
                    while (r.Read()) tableNames.Add(r.GetString(0));
                }

                foreach (var tblName in tableNames)
                {
                    var table = new TableInfo
                    {
                        TableName = tblName,
                        EntityName = GetEntityName(tblName),
                        Columns = new List<ColumnInfo>()
                    };

                    var colCmd = new SqlCommand(
                        "SELECT c.COLUMN_NAME, c.DATA_TYPE, c.CHARACTER_MAXIMUM_LENGTH, c.IS_NULLABLE, " +
                        "COLUMNPROPERTY(OBJECT_ID(c.TABLE_SCHEMA + '.' + c.TABLE_NAME), c.COLUMN_NAME, 'IsIdentity') AS IsIdentity, " +
                        "CASE WHEN pk.COLUMN_NAME IS NOT NULL THEN 1 ELSE 0 END AS IsPK " +
                        "FROM INFORMATION_SCHEMA.COLUMNS c " +
                        "LEFT JOIN (SELECT ku.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc " +
                        "INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku ON tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME " +
                        "WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY' AND tc.TABLE_NAME = @t) pk ON c.COLUMN_NAME = pk.COLUMN_NAME " +
                        "WHERE c.TABLE_NAME = @t ORDER BY c.ORDINAL_POSITION", conn);
                    colCmd.Parameters.AddWithValue("@t", tblName);

                    using (var r = colCmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var col = new ColumnInfo
                            {
                                Name = r.GetString(0),
                                DataType = r.GetString(1),
                                MaxLength = r.IsDBNull(2) ? (int?)null : r.GetInt32(2),
                                IsNullable = r.GetString(3) == "YES",
                                IsIdentity = !r.IsDBNull(4) && Convert.ToInt32(r.GetValue(4)) == 1,
                                IsPrimaryKey = !r.IsDBNull(5) && Convert.ToInt32(r.GetValue(5)) == 1
                            };
                            table.Columns.Add(col);

                            if (col.IsPrimaryKey && table.PrimaryKey == null)
                                table.PrimaryKey = col;
                        }
                    }

                    tables.Add(table);
                }
            }

            return tables;
        }

        // ============ نام Entity نهایی ============
        static string GetEntityName(string tableName)
        {
            // اگه Override داره، همون رو برگردون
            if (EntityOverrides.ContainsKey(tableName))
                return EntityOverrides[tableName];

            return ToEntityName(tableName);
        }

        // ============ تبدیل نام جدول به Entity (مفرد + تمیز) ============
        static string ToEntityName(string tableName)
        {
            var name = tableName;

            if (name.StartsWith("X_"))
                name = name.Substring(2);

            // حذف زیرخط و بزرگ کردن حرف بعدش
            while (name.Contains("_"))
            {
                var idx = name.IndexOf("_");
                if (idx < name.Length - 1)
                {
                    name = name.Substring(0, idx) +
                           char.ToUpper(name[idx + 1]) +
                           name.Substring(idx + 2);
                }
                else
                {
                    name = name.Substring(0, idx);
                }
            }

            // مفرد کردن
            if (name.EndsWith("ies"))
                name = name.Substring(0, name.Length - 3) + "y";
            else if (name.EndsWith("sses"))
                name = name.Substring(0, name.Length - 2);
            else if (name.EndsWith("s") && !name.EndsWith("ss") && !name.EndsWith("us"))
                name = name.Substring(0, name.Length - 1);

            if (!string.IsNullOrEmpty(name))
                name = char.ToUpper(name[0]) + name.Substring(1);

            return name;
        }

        // ============ نام کلاس مفرد (برای LINQ-to-SQL) ============
        static string ToSingularTableName(string tableName)
        {
            // استثناها: این جدول‌ها مفرد هستن، دست نزن
            var exceptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "X_help",
                "X_DigitalFileAccess",
                "X_OrderShipping",
                "X_OrderStatusHistory"
            };

            if (exceptions.Contains(tableName))
                return tableName;

            var name = tableName;
            if (name.EndsWith("ies"))
                name = name.Substring(0, name.Length - 3) + "y";
            else if (name.EndsWith("sses"))
                name = name.Substring(0, name.Length - 2);
            else if (name.EndsWith("s") && !name.EndsWith("ss") && !name.EndsWith("us"))
                name = name.Substring(0, name.Length - 1);
            return name;
        }

        // ============ نام Property توی DataContext ============
        static string GetDbPropertyName(string tableName)
        {
            if (PropertyOverrides.ContainsKey(tableName))
                return PropertyOverrides[tableName];
            return tableName;
        }

        // ============ ساخت Base Controller ============
        static string GenerateBaseController(string ns)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// <auto-generated>");
            sb.AppendLine("// This file was auto-generated. Do not modify manually.");
            sb.AppendLine("// </auto-generated>");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Web.Http;");
            sb.AppendLine("using " + ns + ".Models;");
            sb.AppendLine();
            sb.AppendLine("namespace " + ns + ".Controllers.Base");
            sb.AppendLine("{");
            sb.AppendLine("    public abstract class BaseApiController<TEntity, TViewModel, TKey> : ApiController");
            sb.AppendLine("        where TEntity : class, new()");
            sb.AppendLine("    {");
            sb.AppendLine("        protected DataClassesDatabaseDataContext db = new DataClassesDatabaseDataContext();");
            sb.AppendLine();
            sb.AppendLine("        [HttpGet]");
            sb.AppendLine("        public virtual IHttpActionResult List(int page = 1, int pageSize = 20)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (page < 1) page = 1;");
            sb.AppendLine("            if (pageSize < 1) pageSize = 20;");
            sb.AppendLine("            if (pageSize > 100) pageSize = 100;");
            sb.AppendLine();
            sb.AppendLine("            var query = GetQuery();");
            sb.AppendLine("            var total = query.Count();");
            sb.AppendLine("            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList()");
            sb.AppendLine("                             .Select(MapToViewModel).ToList();");
            sb.AppendLine();
            sb.AppendLine("            return Ok(new {");
            sb.AppendLine("                Total = total,");
            sb.AppendLine("                Page = page,");
            sb.AppendLine("                PageSize = pageSize,");
            sb.AppendLine("                TotalPages = (int)Math.Ceiling((double)total / pageSize),");
            sb.AppendLine("                Data = items");
            sb.AppendLine("            });");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpGet]");
            sb.AppendLine("        public virtual IHttpActionResult Details(TKey id)");
            sb.AppendLine("        {");
            sb.AppendLine("            var entity = FindEntity(id);");
            sb.AppendLine("            if (entity == null) return NotFound();");
            sb.AppendLine("            return Ok(MapToViewModel(entity));");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpPost]");
            sb.AppendLine("        public virtual IHttpActionResult Create(TViewModel model)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (!ModelState.IsValid) return BadRequest(ModelState);");
            sb.AppendLine("            var entity = MapToEntity(model);");
            sb.AppendLine("            InsertEntity(entity);");
            sb.AppendLine("            db.SubmitChanges();");
            sb.AppendLine("            return Ok(MapToViewModel(entity));");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpPut]");
            sb.AppendLine("        public virtual IHttpActionResult Update(TKey id, TViewModel model)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (!ModelState.IsValid) return BadRequest(ModelState);");
            sb.AppendLine("            var entity = FindEntity(id);");
            sb.AppendLine("            if (entity == null) return NotFound();");
            sb.AppendLine("            UpdateEntity(entity, model);");
            sb.AppendLine("            db.SubmitChanges();");
            sb.AppendLine("            return Ok(MapToViewModel(entity));");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        [HttpDelete]");
            sb.AppendLine("        public virtual IHttpActionResult Delete(TKey id)");
            sb.AppendLine("        {");
            sb.AppendLine("            var entity = FindEntity(id);");
            sb.AppendLine("            if (entity == null) return NotFound();");
            sb.AppendLine("            DeleteEntity(entity);");
            sb.AppendLine("            db.SubmitChanges();");
            sb.AppendLine("            return Ok(new { Message = \"Deleted successfully\" });");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        protected abstract IQueryable<TEntity> GetQuery();");
            sb.AppendLine("        protected abstract TEntity FindEntity(TKey id);");
            sb.AppendLine("        protected abstract TViewModel MapToViewModel(TEntity entity);");
            sb.AppendLine("        protected abstract TEntity MapToEntity(TViewModel model);");
            sb.AppendLine("        protected abstract void UpdateEntity(TEntity entity, TViewModel model);");
            sb.AppendLine("        protected abstract void InsertEntity(TEntity entity);");
            sb.AppendLine("        protected abstract void DeleteEntity(TEntity entity);");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        // ============ ساخت ViewModel ============
        static string GenerateViewModel(string ns, TableInfo t)
        {
            var sb = new StringBuilder();
            sb.AppendLine("// <auto-generated>");
            sb.AppendLine("// This file was auto-generated. Do not modify manually.");
            sb.AppendLine("// </auto-generated>");
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine("namespace " + ns + ".ViewModels.Generated");
            sb.AppendLine("{");
            sb.AppendLine("    public class " + t.EntityName + "ViewModel");
            sb.AppendLine("    {");

            foreach (var col in t.Columns)
            {
                sb.AppendLine("        public " + GetCSharpType(col) + " " + col.Name + " { get; set; }");
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        // ============ ساخت Controller ============
        static string GenerateController(string ns, TableInfo t)
        {
            // نام کلاس مفرد برای LINQ-to-SQL
            var singularTable = ToSingularTableName(t.TableName);

            // نام Property توی DataContext (با در نظر گرفتن Overrideها)
            var dbPropertyName = GetDbPropertyName(t.TableName);

            // نام Entity برای ViewModel و Controller
            var entityName = t.EntityName;

            var pk = t.PrimaryKey;
            var pkName = pk != null ? pk.Name : "Id";
            var pkType = pk != null ? GetCSharpType(pk) : "int";
            var routeName = t.TableName.Replace("X_", "").ToLower();

            var sb = new StringBuilder();
            sb.AppendLine("// <auto-generated>");
            sb.AppendLine("// This file was auto-generated. Do not modify manually.");
            sb.AppendLine("// </auto-generated>");
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Web.Http;");
            sb.AppendLine("using " + ns + ".Controllers.Base;");
            sb.AppendLine("using " + ns + ".ViewModels.Generated;");
            sb.AppendLine("using " + ns + ".Models;");
            sb.AppendLine();
            sb.AppendLine("namespace " + ns + ".Controllers.Generated");
            sb.AppendLine("{");
            sb.AppendLine("    [RoutePrefix(\"api/" + routeName + "\")]");
            sb.AppendLine("    public class " + entityName + "Controller : BaseApiController<" + singularTable + ", " + entityName + "ViewModel, " + pkType + ">");
            sb.AppendLine("    {");

            // GetQuery
            sb.AppendLine("        protected override IQueryable<" + singularTable + "> GetQuery()");
            sb.AppendLine("            => db." + dbPropertyName + ".OrderBy(x => x." + pkName + ");");
            sb.AppendLine();

            // FindEntity
            sb.AppendLine("        protected override " + singularTable + " FindEntity(" + pkType + " id)");
            sb.AppendLine("            => db." + dbPropertyName + ".FirstOrDefault(x => x." + pkName + " == id);");
            sb.AppendLine();

            // MapToViewModel
            sb.AppendLine("        protected override " + entityName + "ViewModel MapToViewModel(" + singularTable + " e)");
            sb.AppendLine("            => new " + entityName + "ViewModel");
            sb.AppendLine("            {");
            for (int i = 0; i < t.Columns.Count; i++)
            {
                var col = t.Columns[i];
                var comma = i < t.Columns.Count - 1 ? "," : "";
                sb.AppendLine("                " + col.Name + " = e." + col.Name + comma);
            }
            sb.AppendLine("            };");
            sb.AppendLine();

            // MapToEntity
            sb.AppendLine("        protected override " + singularTable + " MapToEntity(" + entityName + "ViewModel m)");
            sb.AppendLine("            => new " + singularTable);
            sb.AppendLine("            {");
            var entityCols = t.Columns.Where(c => !c.IsIdentity).ToList();
            for (int i = 0; i < entityCols.Count; i++)
            {
                var col = entityCols[i];
                var comma = i < entityCols.Count - 1 ? "," : "";
                sb.AppendLine("                " + col.Name + " = m." + col.Name + comma);
            }
            sb.AppendLine("            };");
            sb.AppendLine();

            // UpdateEntity
            sb.AppendLine("        protected override void UpdateEntity(" + singularTable + " e, " + entityName + "ViewModel m)");
            sb.AppendLine("        {");
            foreach (var col in t.Columns)
            {
                if (col.IsIdentity || col.IsPrimaryKey) continue;
                sb.AppendLine("            e." + col.Name + " = m." + col.Name + ";");
            }
            sb.AppendLine("        }");
            sb.AppendLine();

            // Insert / Delete
            sb.AppendLine("        protected override void InsertEntity(" + singularTable + " e) => db." + dbPropertyName + ".InsertOnSubmit(e);");
            sb.AppendLine();
            sb.AppendLine("        protected override void DeleteEntity(" + singularTable + " e) => db." + dbPropertyName + ".DeleteOnSubmit(e);");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        // ============ تبدیل نوع SQL به C# ============
        static string GetCSharpType(ColumnInfo col)
        {
            string t;
            switch (col.DataType.ToLower())
            {
                case "int": t = "int"; break;
                case "bigint": t = "long"; break;
                case "smallint": t = "short"; break;
                case "tinyint": t = "byte"; break;
                case "bit": t = "bool"; break;
                case "decimal":
                case "numeric":
                case "money":
                case "smallmoney": t = "decimal"; break;
                case "float": t = "double"; break;
                case "real": t = "float"; break;
                case "datetime":
                case "datetime2":
                case "date":
                case "smalldatetime": t = "DateTime"; break;
                case "time": t = "TimeSpan"; break;
                case "uniqueidentifier": t = "Guid"; break;
                case "varbinary":
                case "binary":
                case "image":
                    t = "System.Data.Linq.Binary"; break;
                default: t = "string"; break;
            }

            // System.Data.Linq.Binary خودش reference type هست، ? نمی‌خواد
            if (t == "System.Data.Linq.Binary")
                return t;

            if (col.IsNullable && t != "string" && t != "byte[]")
                return t + "?";

            return t;
        }
    }

    public class TableInfo
    {
        public string TableName { get; set; }
        public string EntityName { get; set; }
        public List<ColumnInfo> Columns { get; set; }
        public ColumnInfo PrimaryKey { get; set; }
    }

    public class ColumnInfo
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public int? MaxLength { get; set; }
        public bool IsNullable { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsPrimaryKey { get; set; }
    }
}