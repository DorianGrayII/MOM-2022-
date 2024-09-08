﻿namespace HutongGames.PlayMaker.Actions
{
    using HutongGames.PlayMaker;
    using PlayMaker.ConditionalExpression;
    using System;
    using System.Runtime.CompilerServices;

    [ActionCategory(ActionCategory.Debug), Tooltip("Checks if the conditional expression Is True or Is False. Breaks the execution of the game if the assertion fails.\nThis is a useful way to check your assumptions. If you expect a certain value use an Assert to make sure!\nOnly runs in Editor.")]
    public class Assert : FsmStateAction, IEvaluatorContext
    {
        [UIHint(UIHint.TextArea), Tooltip("Enter an expression to evaluate.\nExample: (a < b) && c\n$(variable name with spaces)")]
        public FsmString expression;
        [Tooltip("Expected result of the expression.")]
        public AssertType assert;
        [Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;
        private string cachedExpression;

        FsmVar IEvaluatorContext.GetVariable(string name)
        {
            NamedVariable variable = base.Fsm.Variables.GetVariable(name);
            if (variable == null)
            {
                throw new VariableNotFoundException(name);
            }
            return new FsmVar(variable);
        }

        public CompiledAst Ast { get; set; }

        public string LastErrorMessage { get; set; }

        public enum AssertType
        {
            IsTrue,
            IsFalse
        }
    }
}

