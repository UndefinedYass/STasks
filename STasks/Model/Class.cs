using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STasks.Model
{
    public class Class : ProgressDependency
    {

        public Class(Semester semesterParent) :base(semesterParent,false)
        {

        }
        public string Name { get; set; }//todo class names need validation e.g cannot contain dots as that interfers with the URS ruls
        public DateTime ExamDate { get; set; }
        public IEnumerable<Series> Series { get; set; }

    }
}
