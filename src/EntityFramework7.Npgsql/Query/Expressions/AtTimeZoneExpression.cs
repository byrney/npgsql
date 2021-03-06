﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EntityFramework7.Npgsql.Query.Sql;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Utilities;

namespace EntityFramework7.Npgsql.Query.Expressions
{
    public class AtTimeZoneExpression : Expression
    {
        public AtTimeZoneExpression([NotNull] Expression timestampExpression, [NotNull] string timeZone)
        {
            Check.NotNull(timestampExpression, nameof(timestampExpression));
            Check.NotNull(timeZone, nameof(timeZone));

            TimestampExpression = timestampExpression;
            TimeZone = timeZone;
        }

        public Expression TimestampExpression { get; }
        public string TimeZone { get; }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => typeof(DateTime);

        protected override Expression Accept([NotNull] ExpressionVisitor visitor)
        {
            Check.NotNull(visitor, nameof(visitor));

            var specificVisitor = visitor as NpgsqlQuerySqlGenerator;

            return specificVisitor != null
                ? specificVisitor.VisitAtTimeZone(this)
                : base.Accept(visitor);
        }

        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            var newTimestampExpression = visitor.Visit(TimestampExpression);

            return newTimestampExpression != TimestampExpression
                ? new AtTimeZoneExpression(newTimestampExpression, TimeZone)
                : this;
        }

        public override string ToString() => $"{TimestampExpression} AT TIME ZONE {TimeZone}";
    }
}
