using System;
using NHibernate.Mapping.Attributes;

[Class]
public class Customer
{
    [Id(Name = "Id")]
    public virtual Guid Id { get; set; }
    [Property]
    public virtual string Name { get; set; }
}