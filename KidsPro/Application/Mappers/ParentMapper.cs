﻿using Application.Dtos.Response.Account.Parent;
using Application.Dtos.Response.Account.Student;
using Application.Dtos.Response.Certificate;
using Application.Dtos.Response.Course;
using Application.Utils;
using Domain.Entities;

namespace Application.Mappers
{
    public static class ParentMapper
    {
        public static List<StudentResponse> ParentShowStudentList(List<Student> entity)
        {
            var list = new List<StudentResponse>();
            foreach (var x in entity)
            {
                var student = new StudentResponse();
                student.Id = x.Id;
                student.FullName = x.Account.FullName;
                student.Age = DateTime.Now.Year -
                    (x.Account.DateOfBirth != null ? x.Account.DateOfBirth.Value.Year : 0);
                student.Gender=x.Account.Gender?.ToString();
                list.Add(student);
            }
            return list;
        }

        public static StudentDetailResponse ParentShowStudentDetail(Student entity)
        {
            var student = new StudentDetailResponse()
            {
                Account = entity.UserName,
                Id = entity.Id,
                Email = entity.Account.Email,
                FullName = entity.Account.FullName,
                PictureUrl = entity.Account.PictureUrl,
                Gender = entity.Account.Gender?.ToString(),
                DateOfBirth = DateUtils.FormatDateTimeToDateV3(entity.Account.DateOfBirth),
                Status = entity.Account.Status.ToString(),
                CreatedDate = DateUtils.FormatDateTimeToDatetimeV3(entity.Account.CreatedDate),
                Role = entity.Account.Role.Name,
                Age = DateTime.Now.Year -
                    (entity.Account.DateOfBirth != null ? entity.Account.DateOfBirth.Value.Year : 0),

            };

            if (entity.Certificates != null && entity.StudentProgresses != null)
            {
                foreach (var x in entity.Certificates)
                {
                    //List certificate
                    var certificate = new CertificateResponseDto() { title = x.Course.Name, url = x.ResourceUrl };
                    student.StudentsCertificate.Add(certificate);
                }
                student.CertificateTotal = entity.Certificates.Count();

                foreach (var x in entity.StudentProgresses)
                {
                    //List courses
                    var course = new TitleDto() { Id = x.Course.Id, Title = x.Course.Name };
                    student.StudentsCourse.Add(course);
                }
                student.CourseTotal = entity.StudentProgresses.Count();
            }

            return student;
        }

        public static ParentOrderResponse ParentShowContact(Parent entity) => new ParentOrderResponse()
        {
            Email = entity.Account.Email,
            PhoneNumber = entity.PhoneNumber
        };
        
    }
}
