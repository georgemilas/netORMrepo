using Microsoft.Data.OData.Query.SemanticAst;
using System.Web.Http.OData.Builder;
using Microsoft.Data.OData.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Http.OData.Query;
using EM.Collections;
using EM.Api.Core.Models.Exceptions;

namespace EM.Api.Core.OData
{
    public class SelectExpandResult
    {
        public String Name { get; set; }
        public bool IsAllSelected { get; set; }
        public SelectExpandItemCollection Selected { get; set; }
        public SelectExpandItemCollection Expanded { get; set; }
        public SelectExpandItemCollection Query { get; set; }


        /// <summary>
        /// A name that uniquely identifies the APIObject returned, for example  
        ///     for 
        ///         v1/Employees/{id}/Photo 
        ///             the Href of the Photo could be 
        ///         href = p => "Employees/" + id + "/Photo"
        ///     but for
        ///         v1/Employees/{id}
        ///             the Href of the Photo should be 
        ///         Href = p => "Employees/" + ((Employee)p.Parent.Object).ID + "/Photo"  
        ///  so we need to actually build 2 separate types for the same base type of Photo 
        ///  and therefor we would use 2 different names 
        /// </summary>
        public SelectExpandResult(string name)
        {
            Name = name;
            IsAllSelected = true;
            Selected = new SelectExpandItemCollection();
            Expanded = new SelectExpandItemCollection();
            Query = new SelectExpandItemCollection();
        }

        /// <summary>
        /// Parse a path such as /EmployeeDetails/TaxSetup and determine if TaxSetup is requested to be expanded 
        /// </summary>
        public bool IsExpandRequested(string path)
        {
            return IsPathRequestedIn(path, se => se.Expanded);            
        }

        /// <summary>
        /// Parse a path such as /Timecards/Earnings and determine if elements in Earnings have a query associated 
        /// </summary>
        public bool HasQuery(string path)
        {
            return IsPathRequestedIn(path, se => se.Query);
        }

        private bool IsPathRequestedIn(string path, Func<SelectExpandResult, SelectExpandItemCollection> getCollection)
        {
            var names = path.Split('/');
            var collection = getCollection(this);
            foreach (var name in names)
            {
                if (collection == null)
                {
                    return false;
                }
                var exp = collection.FirstOrDefault(e => e.Name == name);
                if (exp != null)
                {
                    collection = exp.SelectExpand != null ? getCollection(exp.SelectExpand) : null;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        //public bool IsSelectedRequested(string name)
        //{
        //    return IsAllSelected || Selected.Exists(e => e.Name == name);
        //}

        public SelectExpandResult GetExpandedResult(string name)
        {
            var expItem = GetExpandedItem(name);
            return expItem != null ? expItem.SelectExpand : null;
        }
        public SelectExpandItem GetExpandedItem(string name)
        {
            var ese = Expanded.FirstOrDefault(s => s.Name == name);
            if (ese != null)
            {
                //order of processing may matter
                ProcessSibling(name, ese, Selected, sse =>
                {
                    if (sse.SelectExpand != null)
                    {
                        ese.SelectExpand.Selected = sse.SelectExpand.Selected;
                        ese.SelectExpand.IsAllSelected = sse.SelectExpand.IsAllSelected;
                    }
                });
                ProcessSibling(name, ese, Query, qse =>
                {
                    ese.RawQuery = qse.RawQuery;
                    if (qse.SelectExpand != null)
                    {
                        ese.SelectExpand.Query = qse.SelectExpand.Query;                        
                    }
                });        
                return ese;
            }
            return null;
        }
        public SelectExpandResult GetSelectedResult(string name)
        {
            var selItem = GetSelectedItem(name);
            return selItem != null ? selItem.SelectExpand : null;
        }
        public SelectExpandItem GetSelectedItem(string name)
        {
            var sse = Selected.FirstOrDefault(s => s.Name == name);
            if (sse != null)
            {
                //order of processing may matter
                ProcessSibling(name, sse, Expanded, ese =>
                {
                    if (ese.SelectExpand != null)
                    {
                        sse.SelectExpand.Expanded = ese.SelectExpand.Expanded;
                    }
                });
                ProcessSibling(name, sse, Query, qse =>
                {
                    sse.RawQuery = qse.RawQuery;
                    if (qse.SelectExpand != null)
                    {
                        sse.SelectExpand.Query = qse.SelectExpand.Query;
                    }
                });
                return sse;
            }
            return null;
        }
        public SelectExpandResult GetQueryResult(string name)
        {
            var qItem = GetQueryItem(name);
            return qItem != null ? qItem.SelectExpand : null;
        }
        public SelectExpandItem GetQueryItem(string name)
        {
            var qse = Query.FirstOrDefault(s => s.Name == name);
            if (qse != null)
            {
                //order of processing may matter
                ProcessSibling(name, qse, Selected, sse =>
                {
                    if (sse.SelectExpand != null)
                    {
                        qse.SelectExpand.Selected = sse.SelectExpand.Selected;
                        qse.SelectExpand.IsAllSelected = sse.SelectExpand.IsAllSelected;
                    }
                });
                ProcessSibling(name, qse, Expanded, ese =>
                {
                    if (ese.SelectExpand != null)
                    {
                        qse.SelectExpand.Expanded = ese.SelectExpand.Expanded;
                    }
                });
                return qse;
            }
            return null;
        }

        private void ProcessSibling(string name, SelectExpandItem main, SelectExpandItemCollection siblingCollection, Action<SelectExpandItem> siblingAction )
        {
            var siblingItem = siblingCollection.FirstOrDefault(s => s.Name == name);
            if (siblingItem != null)
            {
                if (main.SelectExpand == null)
                {
                    main.SelectExpand = siblingItem.SelectExpand;
                }
                siblingAction(siblingItem);                                    
            }
        }

        public string GetHashString()
        {
            string str = Name + IsAllSelected + Selected.GetHashString() + Expanded.GetHashString();
            return str;
        }
        public override int GetHashCode()
        {
            //var hash = Name.GetHashCode() | IsAllSelected.GetHashCode() | Selected.GetHashCode() | Expanded.GetHashCode();
            string hashString = GetHashString();
            var hash = hashString.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is SelectExpandResult)
            {
                var other = obj as SelectExpandResult;
                var eq = Name.Equals(other.Name) && IsAllSelected == other.IsAllSelected && Selected.Equals(other.Selected) && Expanded.Equals(other.Expanded);
                return eq;
            }
            return false;
        }

        //Given these:
        //   $select = Title,HelpPerson/Id,HelpPerson/Parent/Name,HelpPerson/Parent/Id
        //   $expand = HelpPerson/Parent        
        //OData does not accept it but we want it to be OK and to mean  
        //Main       -> Select = Title,HelpPerson  Expand = HelpPerson
        //HelpPerson -> Select = Id,Parent         Expand = Parent
        //Parent     -> Select = Name,Id           Expand = NULL        
        //so, we parse $select ourselfs while $expand is handeled OK by the odata parser itself
        //
        //Here is another example where we have Tasks as List<Task> and * wildchar:
        //   $select = *,Tasks/Description,Tasks/HelpPerson/*,Tasks/HelpPerson/Parent/Name
        //   $expand = Tasks/HelpPerson/Parent
        //Main       -> Select = All (because of *)             Expand = Tasks
        //Task       -> Select = Description,HelpPerson         Expand = HelpPerson
        //HelpPerson -> Select = All (because of HelpPerson/*)  Expand = Parent
        //Parent     -> Select = Name                           Expand = NULL
        /// <summary>
        /// Replaces OData $select parsing with our own parsing for an enhanced model (see also GetFromODataAndCustomQuery)
        /// </summary>
        public static SelectExpandResult HandleRawSelect(string rawSelect)
        {
            if (String.IsNullOrWhiteSpace(rawSelect))
            {
                return null;
            }
            var raw = rawSelect.Replace(" ", "");
            var entries = raw.Split(',');
            var parsedGood = from e in entries where !e.Contains("/") select new SelectExpandItem() { Name = e };
            var res =  new SelectExpandResult(null);
            res.IsAllSelected = parsedGood.FirstOrDefault(e => e.Name == "*") != null;
            var good = (from g in parsedGood where g.Name != "*" select g).ToList();
            
            var withDeep = (from e in entries where e.Contains("/") select e).ToList();
            foreach (var deep in withDeep)
            {
                var deepEntries = deep.Split('/');
                var theGood = good.FirstOrDefault(g => g.Name == deepEntries[0]);
                if (theGood == null)
                {
                    // * is not allowed as the first element of a NavigatioPath ( */Tasks/People -> not allowed)
                    theGood = new SelectExpandItem() { Name = deepEntries[0] };
                    good.Add(theGood);
                }
                var iterGood = theGood;
                
                foreach (var deepGood in deepEntries.Skip(1))
                {
                    if (deepGood == "*")
                    {
                        iterGood.SelectExpand = iterGood.SelectExpand ?? new SelectExpandResult(iterGood.Name);
                        iterGood.SelectExpand.IsAllSelected = true;
                        iterGood.ItemType = SelectExpandItemType.Complex;
                        //does not matter if there is anything after *, we ignore it 
                        //for example in this:  Tasks/*/People  
                        // we know we want all Task elements selected
                        // we could not posibly know what is People an element of so we ignore it (we could also error but we chose not to)                                
                        break;                            
                    }

                    if (iterGood.SelectExpand == null)
                    {
                        iterGood.SelectExpand = new SelectExpandResult(iterGood.Name) { IsAllSelected = false };                        
                        iterGood.ItemType = SelectExpandItemType.Complex;
                    }

                    var deepGoodItem = iterGood.SelectExpand.Selected.FirstOrDefault(g => g.Name == deepGood);
                    if (deepGoodItem == null)
                    {
                        deepGoodItem = new SelectExpandItem() { Name = deepGood };
                        iterGood.SelectExpand.Selected.Add(deepGoodItem);
                    }
                    iterGood = deepGoodItem;
                }                

            }

            res.Selected = new SelectExpandItemCollection();
            res.Selected.AddRange(good);
            return res;
        }


        class PathQuery
        {
            public string Path { get; set; }
            public string Query { get; set; }
        }
        public static SelectExpandResult HandleRawQuery(string rawQuery)
        {
            if (String.IsNullOrWhiteSpace(rawQuery))
            {
                return null;
            }
            var entriesWithQuery = rawQuery.Split(',');
            List<PathQuery> entries = new List<PathQuery>();
            foreach (var e in entriesWithQuery)
            {
                var qix = e.IndexOf("(");
                if (qix > 0)
                {
                    if (e.Substring(e.Length-1) != ")")
                    {
                        throw new ApiParameterParsingException(String.Format("Invalid query syntax; path(query) expected but not close-parens was found: {0} ", e));
                    }
                    entries.Add(new PathQuery() 
                        {
                            Path = e.Substring(0, qix),
                            Query = e.Substring(qix + 1, e.Length - 2 - qix)
                        });
                }
                else
                {
                    throw new ApiParameterParsingException(String.Format("Invalid query syntax; path(query) expected but no open-parens was found: {0} ", e));
                }
            }

            var good = (from e in entries where !e.Path.Contains("/") select new SelectExpandItem() { Name = e.Path, RawQuery = e.Query }).ToList();
            var res = new SelectExpandResult(null);
            
            var withDeep = (from e in entries where e.Path.Contains("/") select e).ToList();
            foreach (var deep in withDeep)
            {
                var deepEntries = deep.Path.Split('/');
                var theGood = good.FirstOrDefault(g => g.Name == deepEntries[0]);
                if (theGood == null)
                {
                    theGood = new SelectExpandItem() { Name = deepEntries[0] };
                    good.Add(theGood);
                }
                var iterGood = theGood;
                foreach (var deepGood in deepEntries.Skip(1))
                {
                    if (iterGood.SelectExpand == null)
                    {
                        iterGood.SelectExpand = new SelectExpandResult(iterGood.Name) { IsAllSelected = false };
                        iterGood.ItemType = SelectExpandItemType.Complex;
                    }

                    var deepGoodItem = iterGood.SelectExpand.Query.FirstOrDefault(g => g.Name == deepGood);
                    if (deepGoodItem == null)
                    {
                        deepGoodItem = new SelectExpandItem() { Name = deepGood };
                        iterGood.SelectExpand.Query.Add(deepGoodItem);
                    }
                    iterGood = deepGoodItem;
                }
                iterGood.RawQuery = deep.Query;

            }

            res.Query = new SelectExpandItemCollection();
            res.Query.AddRange(good);
            return res;
        }
        

        //http://blogs.msdn.com/b/alexj/archive/2013/05/10/parsing-odata-paths-select-and-expand-using-the-odatauriparser.aspx
        //http://www.odata.org/documentation/odata-version-2-0/uri-conventions/ 
        /// <summary>
        /// Map OData SelectExpandClause into our own SelectExpandResult object structure.<para></para> 
        /// This is usualy only the first phase of creating SelectExpandResult (see GetFromODataAndCustomQuery)<para></para>
        /// </summary>
        public static SelectExpandResult HandleSelectExpand(SelectExpandClause selectAndExpand, string name)
        {
            SelectExpandResult res = new SelectExpandResult(name) { IsAllSelected = selectAndExpand.AllSelected };

            var selected = selectAndExpand.SelectedItems;
            foreach (var s in selected)
            {
                if (s is PathSelectItem) //for selected elements $select=
                {
                    PathSelectItem ps = s as PathSelectItem;
                    foreach (var sp in ps.SelectedPath)
                    {
                        if (sp is PropertySegment) //for primitive types
                        {
                            var item = new SelectExpandItem
                            {
                                Name = (sp as PropertySegment).Property.Name,
                                DataType = (sp as PropertySegment).Property.Type.ToString(),
                                ItemType = SelectExpandItemType.Simple
                            };
                            res.Selected.Add(item);
                        }
                        if (sp is NavigationPropertySegment) //for complex types                        
                        {
                            var nps = (sp as NavigationPropertySegment);
                            var item = new SelectExpandItem
                            {
                                Name = nps.NavigationProperty.Name,
                                DataType = nps.NavigationProperty.Type.ToString(),
                                ItemType = SelectExpandItemType.Complex
                            };
                            res.Selected.Add(item);
                        }
                        //var tt = sp.EdmType.TypeKind;
                    }
                }
                if (s is ExpandedNavigationSelectItem) //for expanded elements $expand=
                {
                    ExpandedNavigationSelectItem en = s as ExpandedNavigationSelectItem;
                    foreach (var sp in en.PathToNavigationProperty)
                    {
                        if (sp is NavigationPropertySegment)
                        {
                            var nps = (sp as NavigationPropertySegment);
                            SelectExpandResult aditionalExpand = null;
                            if (en.SelectAndExpand != null)
                            {
                                aditionalExpand = HandleSelectExpand(en.SelectAndExpand, nps.NavigationProperty.Name);
                            }
                            var item = new SelectExpandItem
                            {
                                Name = nps.NavigationProperty.Name,
                                DataType = nps.NavigationProperty.Type.ToString(),
                                ItemType = SelectExpandItemType.Complex,
                                SelectExpand = aditionalExpand
                            };
                            res.Expanded.Add(item);
                            if (!res.IsAllSelected)
                            {
                                res.Selected.Add(item);
                            }
                        }
                        //var tt = sp.EdmType.TypeKind;
                    }
                    //var nm = en.EntitySet.ElementType.Name;
                }
            }

            return res;
        }

        /// <summary>
        /// - Map OData SelectExpandClause into our own SelectExpandResult object structure<para></para>
        /// - Replace $select OData parsing with our own parsing (see HandleRawSelect)<para></para>
        /// - Include our own $query parsing 
        /// </summary>
        public static SelectExpandResult GetFromODataExpandAndCustomSelectAndCustomQuery(ODataQueryOptions queryOptions, Type entityType, string customRawQuery)
        {
            //var se = controller.GetSelectExpandResult<T>(queryOptions);
            var se = new SelectExpandResult(entityType.FullName);
            if (queryOptions != null && queryOptions.SelectExpand != null && queryOptions.SelectExpand.SelectExpandClause != null)
            {
                se = SelectExpandResult.HandleSelectExpand(queryOptions.SelectExpand.SelectExpandClause, entityType.FullName);

                //OData does not accept deep selections like: $select=Title,HelpPerson/Id,HelpPerson/Parent/Name,HelpPerson/Parent/Id&$expand=HelpPerson/Parent
                //so queryOptions.SelectExpand.SelectExpandClause will only have Title,HelpPerson where HelpPerson is in full (with all fields selected) 
                //but we want the above to be OK and so HelpPerson to only have the Id and Parent fields selected and also Parent itself to only have Id and Name
                var ses = SelectExpandResult.HandleRawSelect(queryOptions.SelectExpand.RawSelect);
                if (ses != null)
                {
                    se.IsAllSelected = ses.IsAllSelected;
                    se.Selected.Clear();
                    se.Selected.AddRange(ses.Selected);
                }
            }

            if (!String.IsNullOrWhiteSpace(customRawQuery))
            {
                var qes = SelectExpandResult.HandleRawQuery(customRawQuery);
                se.Query.Clear();
                se.Query.AddRange(qes.Query);
            }

            return se;
        }
        
        
    }

}