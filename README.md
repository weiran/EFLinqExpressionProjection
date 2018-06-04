
# EFLinqExpressionProjecton

This project enables the reuse of LINQ expression logic in projections. This is a fork of [LINQ Expression Projection](https://github.com/AsherBarak/LINQ-Expression-Projection) but with support for async queries from Entity Framework 6 and later.

## Usage

To use, call extension method `AsExpressionProjectable()` on the collection queried, and when
projecting call the extension method `Project<TIn, TResult>(TIn)` (on a field, method or any other
code element) returning a selector of type `Expression<Func<TIn, TResult>>`.
`TIn` and `TResult` can be anything, and they will both be inferred by the compiler meaning that
usages of `Project()` do not have to explicitly specify them.

Example:

```cs
Expression<Func<Project, double>> averageEffectiveAreaExpression =
  proj => proj.Subprojects.Average(sp => sp.Area);

var projects = await context.Projects
  .AsExpressionProjectable()
  .Select(p => new 
  {
    Project = p,
    AverageEffectiveArea = averageEffectiveAreaExpression.Project(p)
  })
  .ToListAsync();
```

## Installation

Find this on NuGet: https://www.nuget.org/packages/EFLinqExpressionProjection
