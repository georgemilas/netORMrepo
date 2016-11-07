using System;
using System.Collections.Generic;
using System.Text;

using ORM.DBFields;
using EM.Collections;
using ORM.render.RenderControls;

namespace ORM.render
{   

    public interface IFormRenderer: IRenderingProvider
    {
        //returns a string or something else, 
        //the client must know how to cast to actual stuff

        /// <summary>
        /// return  
        ///         --read only: <br></br>
        ///             &lt;div id='|table name|' class='ormForm'>&lt;table>\n
        ///         <br></br>
        ///         --read write: <br></br>
        ///             &lt;form id='|table name|' class='ormForm'>&lt;table><br></br>
        ///             &lt;tr>&lt;td colspan=2 class='formInfo'>All fields marked with &lt;em class='require'>*&lt;/em> are required<br></br>
        ///         <br></br>
        ///         --for all:<br></br>    
        ///         &lt;tr id='|tableName_fieldName|'><br></br>
        ///             &lt;td class='dbFieldLabel'>  ... field name<br></br>
        ///             &lt;td>.... rendered field control (see field.renderControl)
        /// 
        /// </summary>
        new Object render();
        new Object render(RenderAttributes renderAttributes);

        /*
        Object render(FBoolean field, RenderAttributes renderAttributes);
        Object render(FChar field, RenderAttributes renderAttributes);
        Object render(FVarchar field, RenderAttributes renderAttributes);
        Object render(FDatetime field, RenderAttributes renderAttributes);
        Object render(FInteger field, RenderAttributes renderAttributes);
        Object render(FFloat field, RenderAttributes renderAttributes); 
        */
    }
}
