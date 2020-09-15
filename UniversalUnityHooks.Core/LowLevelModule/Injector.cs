using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Inject;
using UniversalUnityHooks.Attributes;
using UniversalUnityHooks.Core.Models;

namespace UniversalUnityHooks.Core.LowLevelModule
{
    // TODO: Use CliAssert for null asserts
    public class Injector
    {
        public TypeDefinition TargetType { get; private set; }

        public MethodDefinition TargetMethod { get; private set; }

        public AssemblyDefinition TargetAssembly { get; private set; }

        public FieldDefinition TargetField { get; private set; }

        public TypeDefinition ExecutingType { get; private set; }

        public MethodDefinition ExecutingMethod { get; private set; }

        public AssemblyDefinition ExecutingAssembly { get; private set; }

        public Injector(AssemblyDefinition executingAssembly, AssemblyDefinition targetAssembly)
        {
            ExecutingAssembly = executingAssembly;
            TargetAssembly = targetAssembly;
        }

        private void FindAndSetExecutor(Type targetType, string method = null)
        {
            ExecutingType = ExecutingAssembly.MainModule.GetType(targetType.FullName);
            if (string.IsNullOrWhiteSpace(method))
            {
                return;
            }
            ExecutingMethod = ExecutingType.GetMethod(method);
        }

        private void FindAndSetTarget(Type targetType, string methodOrField = null)
        {
            TargetType = TargetAssembly.MainModule.GetType(targetType.FullName);
            if (string.IsNullOrWhiteSpace(methodOrField))
            {
                return;
            }
            TargetMethod = TargetType.GetMethod(methodOrField);
            if (TargetMethod != null)
            {
                return;
            }
            TargetField = TargetType.GetField(methodOrField);
        }

        public Injector ExecutingHook<T>(Func<T, string> expression, T instance = null) where T : class
        {
            FindAndSetExecutor(typeof(T), expression(instance));
            return this;
        }

        public Injector Target<T>(string target) where T : class
        {
            FindAndSetTarget(typeof(T), target);
            return this;
        }

        public Injector Target<T>(Func<T, string> expression, T instance = null) where T : class
        {
            return Target<T>(expression(instance));
        }

        public Injector Target<T>() where T : class
        {
            FindAndSetTarget(typeof(T), null);
            return this;
        }

        public Injector AddMethod()
        {
            return AddMethod(ExecutingMethod);
        }

        public Injector AddMethod(MethodDefinition method)
        {
            if (TargetType.Methods.Contains(method))
            {
                throw new ArgumentException("The method provided was already found in the target type.");
            }
            TargetType.Methods.Add(method);
            return this;
        }

        public Injector Hook(HookData hookData = null)
        {
            return Hook(ExecutingMethod, hookData);
        }

        public Injector Hook(MethodDefinition hookMethod, HookData hookData = null)
        {
            if (TargetMethod == null)
            {
                throw new NullReferenceException($"{nameof(TargetMethod)} was null, which must be populated if you want to modify the IL code.");
            }

            // TODO: dry coded
            // TODO: Fix 'PassInvokingInstance' to only execute on target instance types
            var flags = InjectFlags.PassInvokingInstance;
            if (TargetMethod.Parameters.Count > 0)
            {
                flags |= InjectFlags.PassParametersRef;
            }
            if (hookMethod.ReturnType.FullName != TargetAssembly.MainModule.TypeSystem.Void.FullName)
            {
                flags |= InjectFlags.ModifyReturn;
            }
            hookData ??= new HookData
            {
                Flags = flags
            };

            var injector = new InjectionDefinition(TargetMethod, hookMethod, hookData.Flags);
            injector.Inject(hookData.StartCode, hookData.Token, hookData.Direction);
            return this;
        }

        public Injector Replace(HookData hookData = null)
        {
            TargetMethod.Body.Instructions.Clear();
            return Hook(ExecutingMethod, hookData);
        }

        /// <summary>
        /// Replaces the method body with a call to the <paramref name="signature"/> delegate. NOTE: This overwrites existing IL code.
        /// </summary>
        /// <param name="signature">The delegate to call once the <see cref="TargetMethod"/> gets executed.</param>
        /// <returns>Returns same instance of <see cref="Injector"/> for chaining easability.</returns>
        public Injector Replace(MethodDefinition hookMethod, HookData hookData = null)
        {
            TargetMethod.Body.Instructions.Clear();
            return Hook(hookMethod, hookData);
        }

        public Injector To(AccessModifier modifiers, bool recursive = false)
        {
            return To(TargetField.Name, modifiers, recursive);
        }

        public Injector To(string member, AccessModifier modifiers, bool recursive = false)
        {
            ChangeAccess(member,
                         modifiers.HasFlag(AccessModifier.Public) && !modifiers.HasFlag(AccessModifier.Private),
                         modifiers.HasFlag(AccessModifier.Virtual),
                         modifiers.HasFlag(AccessModifier.Assignable),
                         recursive);
            return this;
        }
    
        public Injector ChangeAccess(string member, bool makePublic = true, bool makeVirtual = true, bool makeAssignable = true, bool recursive = false)
        {
            TargetType.ChangeAccess(member, makePublic, makeVirtual, makeAssignable, recursive);
            return this;
        }


        public Injector ChangeAccess(bool makePublic = true, bool makeVirtual = true, bool makeAssignable = true, bool recursive = false)
        {
            return ChangeAccess(TargetField.Name, makePublic, makeVirtual, makeAssignable, recursive);
        }
    }
}