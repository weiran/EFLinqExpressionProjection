using System.Collections.Generic;

namespace LinqExpressionProjection.Test.Model
{
    public class Project
    {
        public int Id { get; set; }

        public virtual ICollection<Subproject> Subprojects { get; set; }

        public virtual User CreatedBy { get; set; }

        public virtual User ModifiedBy { get; set; }
    }
}
