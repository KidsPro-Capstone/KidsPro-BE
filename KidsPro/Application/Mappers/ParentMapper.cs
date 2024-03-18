﻿using Application.Dtos.Response.Account.Parent;
using Application.Dtos.Response.Account.Student;
using Application.Dtos.Response.Certificate;
using Application.Dtos.Response.Course;
using Application.Utils;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappers
{
    public class ParentMapper
    {
        public static List<StudentResponseDto> ParentShowListStudent(List<Student> entity)
        {
            var list = new List<StudentResponseDto>();
            foreach (var x in entity)
            {
                var student = new StudentResponseDto();
                student.Id = x.Id;
                student.FullName = x.Account.FullName;
                student.Age = DateTime.Now.Year -
                    (x.Account.DateOfBirth != null ? x.Account.DateOfBirth.Value.Year : 0);
                student.Gender=x.Account?.Gender?.ToString();
                list.Add(student);
            }
            return list;
        }

        public static StudentDetailResponseDto ParentShowStudentDetail(Student entity)
        {
            var student = new StudentDetailResponseDto()
            {
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
                    var _certificate = new CertificateResponseDto() { title = x.Course.Name, url = x.ResourceUrl };
                    student.StudentsCertificate.Add(_certificate);
                }
                student.CertificateTotal = entity.Certificates.Count();

                foreach (var x in entity.StudentProgresses)
                {
                    //List courses
                    var _course = new TitleDto() { Id = x.Course.Id, Title = x.Course.Name };
                    student.StudentsCourse.Add(_course);
                }
                student.CourseTotal = entity.StudentProgresses.Count();
            }

            return student;
        }

        public static ParentOrderResponseDto ParentShowEmailZalo(Parent entity) => new ParentOrderResponseDto()
        {
            Email = entity?.Account?.Email,
            PhoneNumber = entity?.PhoneNumber
        };
        
    }
}
