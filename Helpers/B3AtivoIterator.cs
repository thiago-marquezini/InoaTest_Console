using System.Collections.Generic;

namespace InoaTest_Console.Helpers
{
    class SymbolArgs
    {
        string symbol;
        double refsell;
        double refbuy;

        public SymbolArgs(string Symbol, double RefSell, double RefBuy) 
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

    interface IAbstractSymbols
    {
        Iterator SetupIterator();
    }

    class Collection : IAbstractSymbols
    {
        List<SymbolArgs> Args = new List<SymbolArgs>();

        public Iterator SetupIterator() 
        { 
            return new Iterator(this); 
        }

        public int Count 
        { 
            get { return Args.Count; } 
        }

        public SymbolArgs this[int Index] 
        {
            get { return Args[Index]; } 
            set { Args.Add(value); } 
        }
    }

    interface IAbstractIterator
    {
        SymbolArgs First();

        SymbolArgs Next();

        bool Finished 
        { 
            get; 
        }

        int Index 
        { 
            get; set; 
        }
    }

    class Iterator : IAbstractIterator
    {
        Collection ArgCollection;

        public int Index 
        { 
            get; set; 
        }

        public bool Finished 
        { 
            get { return Index >= ArgCollection.Count; } 
        }

        public Iterator(Collection ArgCollection)
        {
            this.ArgCollection = ArgCollection;
        }

        public SymbolArgs First()
        {
            Index = 0;
            return ArgCollection[Index];
        }

        public SymbolArgs Next()
        {
            Index++;
            if (!Finished)
                return ArgCollection[Index];
            else
                return null;
        }
    }
}
