﻿namespace Application.Configurations;

public static class Constant
{
    //application role
    public const string AdminRole = "Admin";
    public const string ParentRole = "Parent";
    public const string StaffRole = "Staff";
    public const string TeacherRole = "Teacher";
    public const string StudentRole = "Student";
    public const string AdminOrStaffRole = AdminRole + "," + StaffRole;
}