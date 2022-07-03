namespace charlie.bll.interfaces
{
    public interface IRandomNumberProvider
    {
        double Next();
        double Next(double min, double max);
    }
}
