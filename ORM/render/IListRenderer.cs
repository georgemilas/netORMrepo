using System;
using System.Collections.Generic;
using System.Text;

using ORM.DBFields;
using EM.Collections;
using ORM.render.RenderControls;

namespace ORM.render
{
    public interface IListRenderer
    {
        //returns a string or something else, 
        //the client must know how to cast to actual stuff
        bool useAjax { get; set; }

        string name { get; set; }

        //Object render<T>(Table<T> table) where T : TableRow;
        //Object render<T>(Table<T> table, EList<ListAction> actions) where T : TableRow;
        //Object render<T>(Table<T> table, OrderedDictionary<string, string> fields) where T : TableRow;
        //Object render<T>(Table<T> table, OrderedDictionary<string, string> fields, EList<ListAction> actions) where T : TableRow;

        List<RenderingProvider> table { get; set; } 

        Object render();
        Object render(EList<ListAction> actions);
        Object render(OrderedDictionary<string, string> fields);
        Object render(OrderedDictionary<string, string> fields, EList<ListAction> actions);
        
        Object navigationLinks(ListPagingData pagingData);

        void setFieldFormatter(string fieldName, FieldRenderControl.ValueFormatter formatter);
    }
}
