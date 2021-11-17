using System.Collections.Generic;

namespace InoaTest_Console
{
    public class AtivoArgs
    {
        string symbol;
        double refsell;
        double refbuy;

        public AtivoArgs(string Symbol, double RefSell, double RefBuy) 
        { 
            this.symbol  = Symbol; 
            this.refsell = RefSell; 
            this.refbuy  = RefBuy; 
        }
        public string Symbol 
        { 
            get { return symbol; } 
        }
        public double RefSell 
        { 
            get { return refsell; } 
        }
        public double RefBuy 
        { 
            get { return refbuy; } 
        }
    }

    public interface IAbstractSymbols
    {
        Iterator SetupIterator();
    }

    public class Collection : IAbstractSymbols
    {
        List<AtivoArgs> Args = new List<AtivoArgs>();
        public Iterator SetupIterator() { return new Iterator(this); }
        public int Count { get { return Args.Count; } }
        public AtivoArgs this[int Index] 
        {
            get { return Args[Index]; } 
            set { Args.Add(value); } 
        }
    }

    public interface IAbstractIterator
    {
        AtivoArgs First();
        AtivoArgs Next();
        bool Finished { get; }
        int Index { get; set; }
    }

    public class Iterator : IAbstractIterator
    {
        Collection ArgCollection;

        public int Index { get; set; }
        public bool Finished { get { return Index >= ArgCollection.Count; } }

        public Iterator(Collection ArgCollection)
        {
            this.ArgCollection = ArgCollection;
        }

        public AtivoArgs First()
        {
            Index = 0;
            return ArgCollection[Index] as AtivoArgs;
        }

        public AtivoArgs Next()
        {
            Index++;
            if (!Finished)
                return ArgCollection[Index] as AtivoArgs;
            else
                return null;
        }
    }
}
