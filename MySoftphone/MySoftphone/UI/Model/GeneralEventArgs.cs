namespace MySoftphone.UI.Model
{
    public class GeneralEventArgs<T>
    {
        public T Item { get; private set; }

        public GeneralEventArgs(T item)
        {
            Item = item;
        }
    }
}