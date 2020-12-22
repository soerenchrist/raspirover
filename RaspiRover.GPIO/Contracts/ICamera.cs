using System.Threading.Tasks;

namespace RaspiRover.GPIO.Contracts
{
    public interface ICamera
    {
        Task<byte[]> TakeImage();
    }
}