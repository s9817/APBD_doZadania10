using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.dto.request;
using WebApplication1.dto.response;

namespace WebApplication1.db
{
    public interface IEnrollmentDbService
    {
        Models_EF.Enrollment EnrollStudent(EnrollStudentRequest request);

        EnrollStudentResponse PromoteStudent(PromotionRequest request);
    }
}
