public interface IShutdownHandler
{
    void BeginShutdownCharge();
    void UpdateShutdownProgress(float progress);
    void CancelShutdownCharge();
    void CompleteShutdown();
}