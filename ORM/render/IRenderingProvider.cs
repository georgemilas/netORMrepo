using System;
using ORM.DBFields;
using ORM.render.RenderControls;
using EM.Collections;
using System.Collections.Generic;
using ORM.exceptions;

namespace ORM.render
{
    public interface IRenderingProvider: IDictionary<GenericField, FieldRenderControl>
    {
        FieldRenderControl getDefaultRenderControl(GenericField field);
        TableRow table { get; set; }
        RenderAttributes renderAttributes { get; set;}
        BusinessLogicError setTableFromWebForm();
        Object render();
        Object render(RenderAttributes renderAttributes);

        void setFieldFormatter(string fieldName, FieldRenderControl.ValueFormatter formatter);
    }
}
