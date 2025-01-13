using NYaul.IO.FileProvider;

namespace NYaul.HardwareDetection.Linux;

public class RotationalFileReader
{
    private readonly IFileProvider fileSystem;

    public RotationalFileReader(IFileProvider? fileSystem = null)
    {
        this.fileSystem = fileSystem ?? DefaultFileProvider.Instance;
    }

    public bool? IsRotational(string deviceName)
    {
        string rotationalFilePath = $"/sys/block/{deviceName}/queue/rotational";

        if (!fileSystem.FileExists(rotationalFilePath))
        {
            return null;
        }

        string rotationalValue = fileSystem.ReadAllText(rotationalFilePath);
        var positionOfZero = rotationalValue.IndexOf("0");
        var positionOfOne = rotationalValue.IndexOf("1");

        if (positionOfZero == -1 && positionOfOne == -1)
        {
            return null;
        }

        if (positionOfZero == -1)
        {
            // This means that zero is not present. That means that the only value is 1, which means rotational and HDD.
            return true;
        }

        if (positionOfOne == -1)
        {
            // This means that one is not present. That means that the only value is 0, which means non-rotational and SSD.
            return false;
        }

        // For some reason, the file contains both 0 and 1. The first number wins.
        return positionOfZero > positionOfOne;
    }
}