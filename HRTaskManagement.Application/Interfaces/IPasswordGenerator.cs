namespace HRTaskManagement.Application.Interfaces
{
    public interface IPasswordGenerator
    {
        string Generate(int length = 12);
    }
}