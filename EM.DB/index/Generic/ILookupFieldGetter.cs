namespace EM.DB.Index.Generic
{
    public interface ILookupFieldGetter<T>
    {
        object GetField(T data, string fieldName);
    }



    public class GenericPropertyGetter<T> : ILookupFieldGetter<T>
    {
        public object GetField(T data, string fieldName)
        {
            return data.GetType().GetProperty(fieldName).GetValue(data, null);
        }
    }

}