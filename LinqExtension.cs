/*
 * Licensing :
 * LinqExtension is free.
 * LINQExtension is free.
 * The source code is issued under a permissive free license, which means you can modify it as you like, and incorporate it into your own commercial or non-commercial software.
 * Enjoy!
 */
 
/*
 * @author  Ben Heddia Khalil    benhddia-khalil@live.fr
 * @version 1.0, 01/03/2014 
 * @.NET FRAMEWORk 3.5 +
 */


using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LinqKit;


namespace LinqExtension
{
    public static class Extensions 
    {
        private static Type GetTypeFromString(string type)
        {
            object obj;
            var u = Type.GetType("System." + type);
            if (u == typeof(string))
            {
                return typeof(string);
            }
            else
            {
                if (u != null)
                    obj = Activator.CreateInstance(u);
                else
                    return null;
            }

            return obj.GetType();

        }

        private static Expression<Func<T, bool>> BuildPredicate<T>(string member, object value)
        {
            var column = member.ToUpper();
            Boolean verif = false;
            Type PropertyType = null;
            PropertyInfo property = null;
            String PropertyTypeName = String.Empty;
            foreach (var type in typeof(T).GetProperties())
            {
                if (type.ToString().ToUpper().Contains(column.ToUpper()))
                {
                    verif = true;
                    property = type;
                    if (property.PropertyType.Name.Contains("Nullable"))
                    {
                        if (property.PropertyType.ToString().Contains("DateTime"))
                        {
                            PropertyTypeName = "DateTime";
                            PropertyType = GetTypeFromString(PropertyTypeName);
                        }
                        else
                        {
                            if (property.PropertyType.ToString().Contains("Int32"))
                             {
                                    PropertyTypeName = "Int32";
                                    PropertyType = GetTypeFromString(PropertyTypeName);
                             }
                             else
                             {
                                PropertyTypeName = "Double";
                                PropertyType = GetTypeFromString(PropertyTypeName);
                             }
                        }
                    }
                    else
                    {
                        PropertyTypeName = property.PropertyType.Name.ToString();
                        PropertyType = GetTypeFromString(PropertyTypeName);
                    }
                    break;
                }
            }

            if (verif == true)
            {
                var propertyType = property.PropertyType.Name;
                var p = Expression.Parameter(typeof(T), "entity");
                Expression body = p;
                body = Expression.PropertyOrField(body, member);
                switch (PropertyTypeName)
                {
                    case "Int32":
                        {
                            #region traitement des entiers

                            string[] operations = new string[] { ">=", "<=", "<", ">", "=" };
                            string operation = "=";
                            foreach (var op in operations)
                            {
                                if (value.ToString().StartsWith(op))
                                {
                                    operation = op;
                                    value = value.ToString().Replace(op, " ").TrimStart();
                                    break;
                                }
                            }
                            switch (operation)
                            {
                                case ">=":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(body, Expression.Constant(Convert.ToInt32(value), body.Type)), p);
                                    }
                                case ">":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(body, Expression.Constant(Convert.ToInt32(value), body.Type)), p);
                                    }
                                case "<=":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(body, Expression.Constant(Convert.ToInt32(value), body.Type)), p);
                                    }
                                case "<":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.LessThan(body, Expression.Constant(Convert.ToInt32(value), body.Type)), p);
                                    }
                                default:
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.Equal(body, Expression.Constant(Convert.ToInt32(value), body.Type)), p);
                                    }
                            }

                            #endregion
                        }
                    case "String":
                        {
                            #region traitement des string

                            string operation = "Contains";
                            if (value.ToString().StartsWith("%") && !value.ToString().EndsWith("%"))
                            {
                                operation = "StartsWith";
                            }
                            else
                            {
                                if (!value.ToString().StartsWith("%") && value.ToString().EndsWith("%"))
                                {
                                    operation = "EndsWith";
                                }
                                else
                                {
                                    if (value.ToString().StartsWith("="))
                                    {
                                        value = value.ToString().Replace("=", "").TrimStart();
                                    }
                                }
                            }
                            value = value.ToString().Replace("%", "").Trim();

                            Expression Out = Expression.Call(
                                                Expression.Call( // <=== this one is new
                                                    body,
                                                    "ToUpper", null),
                                                operation, null,   //  Param_0 => Param_0.FirstName.ToUpper().Contains("MYVALUE")
                                                Expression.Constant(value.ToString().ToUpper()));

                            return Expression.Lambda<Func<T, bool>>(Out, p);

                            #endregion
                        }
                    case "Decimal":
                        {
                            #region traitement des decimales

                            string[] operations = new string[] { ">=", "<=", "<", ">", "=" };
                            string operation = "=";
                            foreach (var op in operations)
                            {
                                if (value.ToString().StartsWith(op))
                                {
                                    operation = op;
                                    value = value.ToString().Replace(op, " ").TrimStart();
                                    break;
                                }
                            }
                            switch (operation)
                            {
                                case ">=":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(body, Expression.Constant(Convert.ToDecimal(value), body.Type)), p);
                                    }
                                case ">":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(body, Expression.Constant(Convert.ToDecimal(value), body.Type)), p);
                                    }
                                case "<=":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(body, Expression.Constant(Convert.ToDecimal(value), body.Type)), p);
                                    }
                                case "<":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.LessThan(body, Expression.Constant(Convert.ToDecimal(value), body.Type)), p);
                                    }
                                default:
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.Equal(body, Expression.Constant(Convert.ToDecimal(value), body.Type)), p);
                                    }
                            }

                            #endregion
                        }
                    case "DateTime":
                        {
                            #region traitement des dates

                            string[] operations = new string[] { ">=", "<=", "<", ">", "=" };
                            string operation = "=";
                            foreach (var op in operations)
                            {
                                if (value.ToString().StartsWith(op))
                                {
                                    operation = op;
                                    value = value.ToString().Replace(op, " ").TrimStart();
                                    break;
                                }
                            }
                            DateTime objDate;
                            if (string.IsNullOrEmpty(value.ToString()))
                            {
                                DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
                                dtfi.ShortDatePattern = "dd/MM/yyyy";
                                dtfi.DateSeparator = "/";
                                objDate = Convert.ToDateTime("1/1/1900", dtfi);
                                return Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(body, Expression.Constant(objDate, body.Type)), p);
                            }
                            else
                            {
                                DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
                                dtfi.ShortDatePattern = "dd/MM/yyyy";
                                dtfi.DateSeparator = "/";
                                objDate = Convert.ToDateTime(value, dtfi);
                            }

                            switch (operation)
                            {
                                case ">=":
                                    {

                                        return Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(body, Expression.Constant(objDate, body.Type)), p);
                                    }
                                case ">":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(body, Expression.Constant(objDate, body.Type)), p);
                                    }
                                case "<=":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(body, Expression.Constant(objDate, body.Type)), p);
                                    }
                                case "<":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.LessThan(body, Expression.Constant(objDate, body.Type)), p);
                                    }
                                default:
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.Equal(body, Expression.Constant(objDate, body.Type)), p);
                                    }
                            }

                            #endregion
                        }
                    case "Double":
                        {
                            #region traitement les Double


                            string[] operations = new string[] { ">=", "<=", "<", ">", "=" };
                            string operation = "=";
                            foreach (var op in operations)
                            {
                                if (value.ToString().StartsWith(op))
                                {
                                    operation = op;
                                    value = value.ToString().Replace(op, " ").TrimStart();
                                    break;
                                }
                            }
                            switch (operation)
                            {
                                case ">=":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(body, Expression.Constant(Convert.ToDouble(value), body.Type)), p);
                                    }
                                case ">":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(body, Expression.Constant(Convert.ToDouble(value), body.Type)), p);
                                    }
                                case "<=":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(body, Expression.Constant(Convert.ToDouble(value), body.Type)), p);
                                    }
                                case "<":
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.LessThan(body, Expression.Constant(Convert.ToDouble(value), body.Type)), p);
                                    }
                                default:
                                    {
                                        return Expression.Lambda<Func<T, bool>>(Expression.Equal(body, Expression.Constant(Convert.ToDouble(value), body.Type)), p);
                                    }
                            }

                            #endregion
                        }
                    default:
                        {
                            return null;
                        }
                }
            }
            else
            {
                return null;
            }
        }
        //** Pour Enumeration
        public static IEnumerable<T> CollectionToQuery<T>(this IEnumerable<T> list, Dictionary<string, List<string>> dictionary)
        {
            //** Pour IEnumerable
            ParameterExpression pe = Expression.Parameter(typeof(T), "entity");

            Expression left = Expression.Constant(1, typeof(int));
            Expression right = Expression.Constant(1, typeof(int));
            Expression ex = Expression.Equal(left, right);
            List<T> t = list.ToList();
            var Tpredicate = PredicateBuilder.True<T>();
            foreach (var data in dictionary)
            {
                var cle = data.Key;
                var valeur = data.Value;
                var subpredicate = PredicateBuilder.False<T>();
                foreach (var val in valeur)
                {
                    subpredicate = subpredicate.Or(BuildPredicate<T>(cle, val));
                }
                Tpredicate = Tpredicate.And(subpredicate.Expand());  // OK at runtime!
            }

            Func<T, bool> predicate = Tpredicate.Compile();
            t = list.Where(predicate).ToList();


            return t;
        }
        //** Pour IQueryable
        public  static IQueryable<T> CollectionToQuery<T>(this IQueryable<T> entity, Dictionary<string, List<string>> dictionary)
        {//** Pour IQueryable
            ParameterExpression pe = Expression.Parameter(typeof(string), "entity");
            var predicate = PredicateBuilder.True<T>();
            foreach (var data in dictionary)
            {
                var cle = data.Key;
                var valeur = data.Value;
                var subpredicate = PredicateBuilder.False<T>();
                foreach (var val in valeur)
                {
                    subpredicate = subpredicate.Or(BuildPredicate<T>(cle, val));
                }
                predicate = predicate.And(subpredicate.Expand());  // OK at runtime!
            }
            return entity.AsExpandable().Where(predicate);
        }

        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
            {
                return source.Where(predicate);
            }
            else
                return source;
        }

        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> list, bool condition, Func<T, bool> predicate)
        {
            list.FirstOrDefault();
            if (condition)
                return list.Where(predicate);
            else
                return list;
        }
    }
}
