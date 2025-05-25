using System;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Dreamine.MVVM.Core
{
	public static partial class DMContainer
	{
		private static readonly Dictionary<Type, Func<object>> _map = new();

		public static void Register<T>(Func<T> factory) where T : class
			=> _map[typeof(T)] = () => factory();

		public static void RegisterSingleton<T>(T instance) where T : class
			=> _map[typeof(T)] = () => instance;

		public static T Resolve<T>() where T : class
		{
			if (_map.TryGetValue(typeof(T), out var factory))
				return (T)factory();
			throw new InvalidOperationException($"[{typeof(T).Name}] 등록되지 않음.");
		}

		public static object Resolve(Type type)
		{
			if (_map.TryGetValue(type, out var factory))
				return factory();
			throw new InvalidOperationException($"[{type.Name}] 등록되지 않음.");
		}

		/// <summary>
		/// 📦 Assembly 내 Model, Event, Manager, ViewModel, View 자동 등록
		/// </summary>
		public static void AutoRegisterAll(Assembly assembly)
		{
			var types = assembly.GetTypes();

			foreach (var type in types)
			{
				if (!type.IsClass || type.IsAbstract || type.IsGenericType)
					continue;

				// 자동 등록 대상 네이밍 규칙
				bool isTarget =
					type.Name.EndsWith("Model") ||
					type.Name.EndsWith("Event") ||
					type.Name.EndsWith("Manager") ||
					type.Name.EndsWith("ViewModel") ||
					(type.IsSubclassOf(typeof(Window)) || type.IsSubclassOf(typeof(System.Windows.Controls.UserControl)));

				if (!isTarget)
					continue;

				// 이미 등록된 타입은 건너뜀
				if (_map.ContainsKey(type))
					continue;

				// 생성자 분석 (생성자 의존성 주입 지원)
				var ctor = type.GetConstructors()
					.OrderByDescending(c => c.GetParameters().Length)
					.FirstOrDefault();

				if (ctor == null)
					continue;

				_map[type] = () =>
				{
					var args = ctor.GetParameters()
						.Select(p => Resolve(p.ParameterType))
						.ToArray();
					return Activator.CreateInstance(type, args)!;
				};
			}
		}
	}
}
