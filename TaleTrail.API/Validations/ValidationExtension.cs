namespace TaleTrail.API.Validations
{
    public static class ValidationExtensions
    {
        public static void AddError(this Dictionary<string, List<string>> errors, string key, string message)
        {
            if (!errors.ContainsKey(key))
                errors[key] = new List<string>();

            errors[key].Add(message);
        }
    }
}