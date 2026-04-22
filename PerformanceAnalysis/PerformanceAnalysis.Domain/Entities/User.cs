namespace PerformanceAnalysis.Domain.Entities;

public partial class User
{
    public int Id { get; set; }
    public string Login { get; set; } = null!;
    public string Passwordhash { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Firstname { get; set; } = null!;
    public string? Middlename { get; set; }
    public string Lastname { get; set; } = null!;
    public string Role { get; set; } = null!;
    public DateTime Createdat { get; set; }

    public virtual ICollection<Refreshtoken> Refreshtokens { get; set; } = new List<Refreshtoken>();
    public virtual Student? Student { get; set; }
}

public partial class Student
{
    public int Id { get; set; }
    public string Phone { get; set; } = null!;
    public string Vkprofilelink { get; set; } = null!;
    public string? Avatarpath { get; set; }
    public int Userid { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
}

public partial class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Directionid { get; set; }
    public int Courseid { get; set; }
    public int Projectid { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}

public partial class Refreshtoken
{
    public int Id { get; set; }
    public string Tokenhash { get; set; } = null!;
    public int Userid { get; set; }
    public DateTime Createdat { get; set; }
    public DateTime Expiresat { get; set; }
    public DateTime? Revokedat { get; set; }

    public virtual User User { get; set; } = null!;
}
