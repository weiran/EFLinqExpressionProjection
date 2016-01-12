using System;
using System.Linq.Expressions;

// ReSharper disable InconsistentNaming

namespace LinqExpressionProjection.Test.Model
{
    public class Subproject
    {
        public int Id { get; set; }

        public Project Project { get; set; }

        public int Area { get; set; }

        public static readonly Expression<Func<Subproject, string>> StaticFieldOnType_BasicExpression = subProject => "StaticFieldOnType_BasicExpression - Area: " + subProject.Area;
    }
}
