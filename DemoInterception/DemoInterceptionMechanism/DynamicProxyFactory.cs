using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Linq;

namespace DemoInterceptionMechanism
{
    public class DynamicProxyFactory
    {
        AssemblyBuilder assemblyBuilder;
        ModuleBuilder moduleBuilder;

        public DynamicProxyFactory()
        {
            //初始化AssemblyName的一个实例
            AssemblyName assemblyName = new AssemblyName();
            //设置程序集的名称
            assemblyName.Name = "DynamicProxyMechanism";
            //动态的在当前应用程序域创建一个应用程序集
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            //动态在程序集内创建一个模块
            moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicProxyMechanism");
        }

        /// <summary>
        /// 类拦截
        /// </summary>
        /// <param name="targetClass">目标类，被代理类</param>
        /// <param name="proxyMethodInfo">目标方法，被代理方法</param>
        /// <param name="interceptorType">拦截器</param>
        /// <returns></returns>
        public Type CreateClassProxy<Interceptor>(Type targetClass, MethodInfo proxyMethodInfo)
            where Interceptor : IInterceptor
        {
            Type interceptorType = typeof(Interceptor);
            MethodInfo beforeMethodInfo = interceptorType.GetMethod("Before");
            MethodInfo afterMethodInfo = interceptorType.GetMethod("After");

            //动态的在模块内创建一个类
            TypeBuilder typeBuilder = moduleBuilder.DefineType(targetClass.Name + "ClassProxyByEmit", TypeAttributes.Public | TypeAttributes.Class, targetClass, null);

            //定义字段
            FieldBuilder interceptorFieldBuilder = typeBuilder.DefineField("interceptor", interceptorType, FieldAttributes.Public);

            #region 构造函数

            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                    CallingConventions.ExplicitThis, new Type[0]);
            ILGenerator ilG = constructorBuilder.GetILGenerator();

            //调用父构造函数
            ilG.Emit(OpCodes.Ldarg_0);
            ilG.Emit(OpCodes.Call, typeof(DoubleCalcService).GetConstructor(new Type[0]));

            //实例化拦截器
            ilG.Emit(OpCodes.Ldarg_0);
            ilG.Emit(OpCodes.Newobj, interceptorType.GetConstructor(new Type[0]));
            ilG.Emit(OpCodes.Stfld, interceptorFieldBuilder);

            //构造函数返回
            ilG.Emit(OpCodes.Ret);

            #endregion

            #region 重载目标方法

            //动态的为类里创建一个方法
            MethodBuilder doubleMethodBuilder = typeBuilder.DefineMethod(proxyMethodInfo.Name,
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                proxyMethodInfo.ReturnType,
                proxyMethodInfo.GetParameters().Select(p => p.ParameterType).ToArray());

            //重载
            typeBuilder.DefineMethodOverride(doubleMethodBuilder, proxyMethodInfo);

            //得到该方法的ILGenerator
            ilG = doubleMethodBuilder.GetILGenerator();
            ilG.DeclareLocal(proxyMethodInfo.ReturnType);

            // 调用拦截器Before方法
            ilG.Emit(OpCodes.Ldarg_0);
            ilG.Emit(OpCodes.Ldfld, interceptorFieldBuilder);
            ilG.Emit(OpCodes.Ldstr, proxyMethodInfo.Name);
            ilG.Emit(OpCodes.Ldarg_1);
            ilG.Emit(OpCodes.Callvirt, beforeMethodInfo);

            // 调用真实类型Double方法
            ilG.Emit(OpCodes.Ldarg_0);
            int i = 1;
            foreach (var paramInfo in proxyMethodInfo.GetParameters())
            {
                ilG.Emit(OpCodes.Ldarg, i++);
            }
            ilG.Emit(OpCodes.Call, proxyMethodInfo);
            ilG.Emit(OpCodes.Stloc_0);


            // 调用拦截器After方法
            ilG.Emit(OpCodes.Ldarg_0);
            ilG.Emit(OpCodes.Ldfld, interceptorFieldBuilder);
            ilG.Emit(OpCodes.Ldstr, proxyMethodInfo.Name);
            i = 1;
            foreach (var paramInfo in proxyMethodInfo.GetParameters())
            {
                ilG.Emit(OpCodes.Ldarg, i++);
            }
            ilG.Emit(OpCodes.Ldloc_0);
            ilG.Emit(OpCodes.Callvirt, afterMethodInfo);

            // 返回结果
            ilG.Emit(OpCodes.Ldloc_0);
            ilG.Emit(OpCodes.Ret);

            #endregion

            //创建Emit构建的类型
            var proxyType = typeBuilder.CreateType();
            return proxyType;
        }

        /// <summary>
        /// 接口拦截
        /// </summary>
        /// <param name="targetInterface">目标接口，被代理接口</param>
        /// <param name="implementClass">实现类</param>
        /// <param name="proxyMethodInfo">目标方法，被代理方法</param>
        /// <param name="interceptorType">拦截器</param>
        /// <returns></returns>
        public Type CreateInterfaceProxy<Interceptor>(Type targetInterface, Type implementClass, MethodInfo proxyMethodInfo)
            where Interceptor : IInterceptor
        {
            Type interceptorType = typeof(Interceptor);
            MethodInfo beforeMethodInfo = interceptorType.GetMethod("Before");
            MethodInfo afterMethodInfo = interceptorType.GetMethod("After");

            //动态的在模块内创建一个类
            TypeBuilder typeBuilder = moduleBuilder.DefineType(implementClass.Name + "InterfaceProxyByEmit", TypeAttributes.Public | TypeAttributes.Class, null, new Type[] { targetInterface });

            //定义字段
            FieldBuilder interceptorFieldBuilder = typeBuilder.DefineField("interceptor", interceptorType, FieldAttributes.Public);
            FieldBuilder targetFieldBuilder = typeBuilder.DefineField("target", implementClass, FieldAttributes.Public);

            #region 构造函数

            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
                    CallingConventions.ExplicitThis, new Type[0]);
            ILGenerator ilG = constructorBuilder.GetILGenerator();

            //实例化拦截器
            ilG.Emit(OpCodes.Ldarg_0);
            ilG.Emit(OpCodes.Newobj, interceptorType.GetConstructor(new Type[0]));
            ilG.Emit(OpCodes.Stfld, interceptorFieldBuilder);

            //实例化目标类
            ilG.Emit(OpCodes.Ldarg_0);
            ilG.Emit(OpCodes.Newobj, implementClass.GetConstructor(new Type[0]));
            ilG.Emit(OpCodes.Stfld, targetFieldBuilder);

            //构造函数返回
            ilG.Emit(OpCodes.Ret);

            #endregion

            #region 重载目标方法

            //动态的为类里创建一个方法
            MethodBuilder doubleMethodBuilder = typeBuilder.DefineMethod(proxyMethodInfo.Name,
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                proxyMethodInfo.ReturnType,
                proxyMethodInfo.GetParameters().Select(p => p.ParameterType).ToArray());

            //重载
            typeBuilder.DefineMethodOverride(doubleMethodBuilder, proxyMethodInfo);

            //得到该方法的ILGenerator
            ilG = doubleMethodBuilder.GetILGenerator();
            ilG.DeclareLocal(proxyMethodInfo.ReturnType);

            // 调用拦截器Before方法
            ilG.Emit(OpCodes.Ldarg_0);
            ilG.Emit(OpCodes.Ldfld, interceptorFieldBuilder);
            ilG.Emit(OpCodes.Ldstr, proxyMethodInfo.Name);
            ilG.Emit(OpCodes.Ldarg_1);
            ilG.Emit(OpCodes.Callvirt, beforeMethodInfo);

            // 调用真实类型Double方法
            ilG.Emit(OpCodes.Ldarg_0);
            ilG.Emit(OpCodes.Ldfld, targetFieldBuilder);
            int i = 1;
            foreach (var paramInfo in proxyMethodInfo.GetParameters())
            {
                ilG.Emit(OpCodes.Ldarg, i++);
            }
            ilG.Emit(OpCodes.Callvirt, proxyMethodInfo);
            ilG.Emit(OpCodes.Stloc_0);


            // 调用拦截器After方法
            ilG.Emit(OpCodes.Ldarg_0);
            ilG.Emit(OpCodes.Ldfld, interceptorFieldBuilder);
            ilG.Emit(OpCodes.Ldstr, proxyMethodInfo.Name);
            i = 1;
            foreach (var paramInfo in proxyMethodInfo.GetParameters())
            {
                ilG.Emit(OpCodes.Ldarg, i++);
            }
            ilG.Emit(OpCodes.Ldloc_0);
            ilG.Emit(OpCodes.Callvirt, afterMethodInfo);

            // 返回结果
            ilG.Emit(OpCodes.Ldloc_0);
            ilG.Emit(OpCodes.Ret);

            #endregion

            //创建Emit构建的类型
            var proxyType = typeBuilder.CreateType();
            return proxyType;
        }

    }
}
