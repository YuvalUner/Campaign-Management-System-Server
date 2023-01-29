namespace DAL.Models;

public class JobAssignmentParams
{
    public IEnumerable<string>? UserEmailAddress { get; set; }
    public IEnumerable<int>? Salaries { get; set; }
}