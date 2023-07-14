namespace Qkmaxware.Languages.C;

public class OperatorSet {
    #region Arithmetic
    public List<AddOperator> Addition {get; private set;} = new List<AddOperator>();
    public List<SubOperator> Subtraction {get; private set;} = new List<SubOperator>();
    public List<MulOperator> Multiplication {get; private set;} = new List<MulOperator>();
    public List<DivOperator> Division {get; private set;} = new List<DivOperator>();
    public List<RemOperator> Remainder {get; private set;} = new List<RemOperator>();
    #endregion

    #region Comparison
    public List<LtOperator> LessThan {get; private set;} = new List<LtOperator>();
    public List<GtOperator> GreaterThan {get; private set;} = new List<GtOperator>();
    public List<EqOperator> Equality {get; private set;} = new List<EqOperator>();
    public List<NeqOperator> Inequality {get; private set;} = new List<NeqOperator>();
    #endregion

    #region Bitwise 
    public List<AndOperator> And {get; private set;} = new List<AndOperator>();
    public List<OrOperator> Or {get; private set;} = new List<OrOperator>();
    public List<XorOperator> Xor {get; private set;} = new List<XorOperator>();
    #endregion

    #region Accessors
    #endregion
}
