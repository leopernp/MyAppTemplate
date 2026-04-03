namespace DamayanFS.Contract.Helpers;

public class CustomValidateResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }

    public List<string> Errors { get; set; } = new List<string>();

    public CustomValidateResult AddError(string errorMessage)
    {
        IsValid = false;
        Errors.Add(errorMessage);
        return this;
    }

    public CustomValidateResult(bool isValid, string? errorMessage = null)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }
}
