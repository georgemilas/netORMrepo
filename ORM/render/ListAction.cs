using System;
using System.Collections.Generic;
using System.Text;

using EM.Collections;

namespace ORM.render
{
    /// <summary>
    /// how to render an "action button" for a row on a grid
    ///   - either as a button (label+url)
    ///   - or as your own HTML
    /// </summary>
    public class ListAction
    {
        public enum Position { Left, Right };

        public string label;
        public string url;
        public EList<string> aditionalFields;
        public string html;
        public Position position;

        public ListAction(string url, string label) : this(url, label, new EList<string>(), ListAction.Position.Right) { }
        public ListAction(string url, string label, EList<string> aditionalFields) : this(url, label, aditionalFields, ListAction.Position.Right) { }
        public ListAction(string url, string label, EList<string> aditionalFields, ListAction.Position position)
        {
            this.url = url;
            this.label = label;
            if (aditionalFields != null)
            {
                this.aditionalFields = aditionalFields;
            }
            else
            {
                this.aditionalFields = new EList<string>();
            }
            this.html = null;
            this.position = position;
        }
        public ListAction(string html) : this(html, new EList<string>(), ListAction.Position.Right) { }
        public ListAction(string html, EList<string> aditionalFields) : this(html, aditionalFields, ListAction.Position.Right) { }
        public ListAction(string html, EList<string> aditionalFields, ListAction.Position position)
        {
            this.html = html;
            this.url = null;
            this.label = null;
            if (aditionalFields != null)
            {
                this.aditionalFields = aditionalFields;
            }
            else
            {
                this.aditionalFields = new EList<string>();
            }
            this.position = position;
        }
    }

}
