using FirstWave.Unity.Gui.Data;
using System;
using System.Linq.Expressions;

namespace FirstWave.Unity.Data
{
	public class NotifyableObject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		#region INotifyPropertyChanged

		public void NotifyPropertyChange<TProperty>(Expression<Func<TProperty>> property)
		{
			var memberExpr = GetMemberExpression<TProperty>(property);

			RaisePropertyChanged(memberExpr.Member.Name);
		}

		public void RaisePropertyChanged(string propName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propName));
		}

		private MemberExpression GetMemberExpression<TProperty>(Expression<Func<TProperty>> property)
		{
			var lambda = (LambdaExpression)property;

			MemberExpression memberExpr;
			if (lambda.Body is UnaryExpression)
			{
				var unaryExpr = (UnaryExpression)lambda.Body;
				memberExpr = (MemberExpression)unaryExpr.Operand;
			}
			else memberExpr = (MemberExpression)lambda.Body;

			return memberExpr;
		}

		#endregion
	}
}
