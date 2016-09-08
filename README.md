
# EFLinqExpressionProjecton

This assembly enables ruse of LINQ logic in projections.

## Usage

To use, call extension method `AsExpressionProjectable()` on the collection queried, and when
projecting call the extension method `Project<TIn, TResult>(TIn)` (on a field, method or any other
code element) returning a selector of type `Expression<Func<TIn, TResult>>`.
`TIn` and `TResult` can be anything, and they will both be inferred by the compiler meaning that
usages of `Project()` do not have to explicitly specify them.

Example:
```cs
var projects = await context.Projects
  .AsExpressionProjectable()
  .Select(p => new 
  {
    Project = p,
    AEA = AverageEffectiveAreaExpression.Project(p)
  })
  .ToListAsync();
```

## Installation

Find this on NuGet: https://www.nuget.org/packages/EFLinqExpressionProjection
