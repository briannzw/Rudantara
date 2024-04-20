public interface IRequestResponse
{
    public void Send(string prompt);
    public void Receive(string response);
}
