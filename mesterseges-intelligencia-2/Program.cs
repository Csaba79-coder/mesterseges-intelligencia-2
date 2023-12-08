using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AbsztraktÁllapot k = new KannibálokÉsSzerzetesek();
            Csúcs start = new Csúcs(k);
            GráfKereső kereső = new BackTrack(start, 8, true);
            kereső.megoldásKiírása(kereső.Keresés());
            Console.ReadLine();
        }
    }
    // (A,k,C,O,PRE,POST)
    // (A,k,RészCélok,C,O,PRE+Stratégia,POST)
    abstract class AbsztraktÁllapot : ICloneable
    {
        public virtual object Clone() { return MemberwiseClone(); }
        public abstract bool ÁllapotE();
        public abstract int RészCélokSzáma();
        public abstract bool RészCélE(int i);
        public abstract int TranzakciókSzáma(int részCélIndexe);
        public abstract bool Tranzakció(int részCélIndexe, int i);
        public abstract bool CélÁllapotE();
        public abstract int OperátorokSzáma();
        public abstract bool SzuperOperátor(int i);
        //public abstract void SetRészCél(int kövRészCélIndexe);

    }
    // 3 szerzetes 3 kannibál
    // k: mindenki jobb oldalon van
    // részcél_0: 2-2 szerzetes, illetve kannibál a bal oldalon
    // részcél_1: 3 szerzetes a bal oldalon
    // cél: mindenki a jobb oldalon
    class KannibálokÉsSzerzetesek : AbsztraktÁllapot
    {
        int kb, kj, szb, szj;
        string cs;
        public KannibálokÉsSzerzetesek()
        {
            szb = 0; kb = 0;
            szj = 3; kj = 3;
            cs = "jobb";
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is KannibálokÉsSzerzetesek)) return false;
            KannibálokÉsSzerzetesek másik = obj as KannibálokÉsSzerzetesek;
            return this.kb == másik.kb && this.kj == másik.kj &&
                    this.szb == másik.szb && this.szj == másik.szj &&
                    this.cs == másik.cs;
        }
        public override string ToString()
        {
            return "szb: " + szb + "kb: " + kb +
                   "szj: " + szj + "kj: " + kj + "cs: " + cs;
        }

        public override bool ÁllapotE()
        {
            return (kb <= szb || szb == 0) &&
                   (kj <= szj || szj == 0);
        }
        public override int RészCélokSzáma()
        {
            return 2;
        }
        public override bool RészCélE(int i)
        {
            switch (i)
            {
                // részcél_0: 2-2 szerzetes, illetve kannibál a bal oldalon
                case 0: return kb == 2 && szb == 2;
                // részcél_1: 3 szerzetes a bal oldalon
                case 1: return szb == 3;
                default: return false;
            }
        }
        public override int TranzakciókSzáma(int részCélIndexe)
        {
            // 2x 0-2, 0-1, 0-2, mindkét esetben
            // 3x 0-2, 0-1, mindkét esetben
            // 1x 1-1, 2-0, csak a másodiknál
            switch (részCélIndexe)
            {
                case 0: return 2;
                case 1: return 3;
                default: return 0;
            }
        }
        public override bool Tranzakció(int részCélIndexe, int i)
        {
            // 2x T1: 0-2, 0-1, 0-2, mindkét esetben
            // 3x T2: 0-2, 0-1, mindkét esetben
            // 1x T3: 1-1, 2-0, csak a másodiknál
            switch (részCélIndexe)
            {
                case 0:
                    switch (i)
                    {
                        case 0: return T1();
                        case 1: return T2();
                    }
                    break;
                case 1:
                    switch (i)
                    {
                        case 0: return T1();
                        case 1: return T2();
                        case 2: return T3();
                    }
                    break;
            }
            return false;
        }
        public override bool CélÁllapotE()
        {
            return kb == 3 && szb == 3;
        }
        public override int OperátorokSzáma()
        {
            return 5;
        }
        public override bool SzuperOperátor(int i)
        {
            switch (i)
            {
                case 0: return átvisz(0, 2);
                case 1: return átvisz(0, 1);
                case 2: return átvisz(2, 0);
                case 3: return átvisz(1, 0);
                case 4: return átvisz(1, 1);
                default: return false;
            }
        }
        private bool átvisz(int sz, int k)
        {
            if (!preÁtvisz(sz, k)) return false;
            if (cs == "bal")
            {
                szb -= sz;
                szj += sz;
                kb -= k;
                kj += k;
                cs = "jobb";
            }
            else
            {
                szb += sz;
                szj -= sz;
                kb += k;
                kj -= k;
                cs = "bal";
            }
            if (ÁllapotE()) return true; else return false;
        }
        private bool preÁtvisz(int sz, int k)
        {
            if (cs == "bal") return szb >= sz && kb >= k;
            else return szj >= sz && kj >= k;
        }
        // 2x T1: 0-2, 0-1, 0-2, mindkét esetben
        // 3x T2: 0-2, 0-1, mindkét esetben
        // 1x T3: 1-1, 2-0, csak a másodiknál
        private bool T1()
        {
            return SzuperOperátor(0) &&
                   SzuperOperátor(1) &&
                   SzuperOperátor(0);
        }
        // 3x T2: 0-2, 0-1, mindkét esetben
        private bool T2()
        {
            return SzuperOperátor(0) &&
                   SzuperOperátor(1);
        }
        // 1x T3: 1-1, 2-0, csak a másodiknál
        private bool T3()
        {
            return SzuperOperátor(4) &&
                   SzuperOperátor(2);
        }
        //public abstract void SetRészCél(int kövRészCélIndexe);
    }


    //8.5.1
    // <summary>
    /// A csúcs tartalmaz egy állapotot, a csúcs mélységét, és a csúcs szülőjét.
    /// Így egy csúcs egy egész utat reprezentál a start csúcsig.
    /// </summary>
    class Csúcs
    {
        // A csúcs tartalmaz egy állapotot, a mélységét és a szülőjét
        AbsztraktÁllapot állapot;
        int mélység;
        Csúcs szülő; // A szülőkön felfelé haladva a start csúcsig jutok.
                     // Konstruktor:
                     // A belső állapotot beállítja a start csúcsra.
                     // A hívó felelőssége, hogy a kezdő állapottal hívja meg.
                     // A start csúcs mélysége 0, szülője nincs.
        public Csúcs(AbsztraktÁllapot kezdőÁllapot)
        {
            állapot = kezdőÁllapot;
            mélység = 0;
            szülő = null;
        }
        // Egy új gyermek csúcsot készít.
        // Erre még meg kell hívni egy alkalmazható operátor is, csak azután lesz kész.
        public Csúcs(Csúcs szülő)
        {
            állapot = (AbsztraktÁllapot)szülő.állapot.Clone();
            mélység = szülő.mélység + 1;
            this.szülő = szülő;
        }
        public Csúcs GetSzülő() { return szülő; }
        public int GetMélység() { return mélység; }
        public bool TerminálisCsúcsE() { return állapot.CélÁllapotE(); }
        public int OperátorokSzáma() { return állapot.OperátorokSzáma(); }
        public bool SzuperOperátor(int i) { return állapot.SzuperOperátor(i); }
        public int TranzakciókSzáma(int részCélIndexe)
        {
            return állapot.TranzakciókSzáma(részCélIndexe);
        }
        public bool Tranzakció(int részCélIndexe, int ti)
        {
            return állapot.Tranzakció(részCélIndexe, ti);
        }
        public bool RészCélE(int részCélIndexe)
        {
            return állapot.RészCélE(részCélIndexe);
        }
        public override bool Equals(Object obj)
        {
            Csúcs cs = (Csúcs)obj;
            return állapot.Equals(cs.állapot);
        }
        public override int GetHashCode() { return állapot.GetHashCode(); }
        public override String ToString() { return állapot.ToString(); }
        // Alkalmazza az összes alkalmazható operátort.
        // Visszaadja az így előálló új csúcsokat.
        public List<Csúcs> Kiterjesztes()
        {
            List<Csúcs> újCsúcsok = new List<Csúcs>();
            for (int i = 0; i < OperátorokSzáma(); i++)
            {
                // Új gyermek csúcsot készítek.
                Csúcs újCsúcs = new Csúcs(this);
                // Kiprobálom az i.dik alapoperátort. Alkalmazható?
                if (újCsúcs.SzuperOperátor(i))
                {
                    // Ha igen, hozzáadom az újakhoz.
                    újCsúcsok.Add(újCsúcs);
                }
            }
            return újCsúcsok;
        }
    }

    // 8.6.1
    /// <summary>
    /// Minden gráfkereső algoritmus őse.
    /// A gráfkeresőknek csak a Keresés metódust kell megvalósítaniuk.
    /// Ez visszaad egy terminális csúcsot, ha talált megoldást, egyébként null értékkel tér vissza.
    /// A terminális csúcsból a szülő referenciákon felfelé haladva áll elő a megoldás.
    /// </summary>
    abstract class GráfKereső
    {
        private Csúcs startCsúcs; // A start csúcs csúcs.
                                  // Minden gráfkereső a start csúcsból kezd el keresni.
        public GráfKereső(Csúcs startCsúcs)
        {
            this.startCsúcs = startCsúcs;
        }
        // Jobb, ha a start csúcs privát, de a gyermek osztályok lekérhetik.
        protected Csúcs GetStartCsúcs() { return startCsúcs; }
        /// Ha van megoldás, azaz van olyan út az állapottér gráfban,
        /// ami a start csúcsból egy terminális csúcsba vezet,
        /// akkor visszaad egy megoldást, egyébként null.
        /// A megoldást egy terminális csúcsként adja vissza.
        /// Ezen csúcs szülő referenciáin felfelé haladva adódik a megoldás fordított sorrendben.
        public abstract Csúcs Keresés();
        /// <summary>
        /// Kiíratja a megoldást egy terminális csúcs alapján.
        /// Feltételezi, hogy a terminális csúcs szülő referenciáján felfelé haladva eljutunk a start csúcshoz.
        /// A csúcsok sorrendjét megfordítja, hogy helyesen tudja kiírni a megoldást.
        /// Ha a csúcs null, akkor kiírja, hogy nincs megoldás.
        /// </summary>
        /// <param name="egyTerminálisCsúcs">
        /// A megoldást képviselő terminális csúcs vagy null.
        /// </param>
        public void megoldásKiírása(Csúcs egyTerminálisCsúcs)
        {
            if (egyTerminálisCsúcs == null)
            {
                Console.WriteLine("Nincs megoldás");
                return;
            }
            // Meg kell fordítani a csúcsok sorrendjét.
            Stack<Csúcs> megoldás = new Stack<Csúcs>();
            Csúcs aktCsúcs = egyTerminálisCsúcs;
            while (aktCsúcs != null)
            {
                megoldás.Push(aktCsúcs);
                aktCsúcs = aktCsúcs.GetSzülő();
            }
            // Megfordítottuk, lehet kiírni.
            foreach (Csúcs akt in megoldás) Console.WriteLine(akt);
        }
    }

    // 8.7.1
    /// <summary>
    /// A backtrack gráfkereső algoritmust megvalósító osztály.
    /// A három alap backtrack algoritmust egyben tartalmazza. Ezek
    /// - az alap backtrack
    /// - mélységi korlátos backtrack
    /// - emlékezetes backtrack
    /// Az ág-korlátos backtrack nincs megvalósítva.
    /// </summary>
    class BackTrack : GráfKereső
    {
        int korlát; // Ha nem nulla, akkor mélységi korlátos kereső.
        bool emlékezetes; // Ha igaz, emlékezetes kereső.
        public BackTrack(Csúcs startCsúcs, int korlát, bool emlékezetes) : base(startCsúcs)
        {
            this.korlát = korlát;
            this.emlékezetes = emlékezetes;
        }
        // nincs mélységi korlát, se emlékezet
        public BackTrack(Csúcs startCsúcs) : this(startCsúcs, 0, false) { }
        // mélységi korlátos kereső
        public BackTrack(Csúcs startCsúcs, int korlát) : this(startCsúcs, korlát, false) { }
        // emlékezetes kereső
        public BackTrack(Csúcs startCsúcs, bool emlékezetes) : this(startCsúcs, 0, emlékezetes) { }
        // A keresés a start csúcsból indul.
        // Egy terminális csúcsot ad vissza. A start csúcsból el lehet jutni ebbe a terminális csúcsba.
        // Ha nincs ilyen, akkor null értéket ad vissza.
        public override Csúcs Keresés()
        {
            return Keresés(GetStartCsúcs(), 0);
        }
        // A kereső algoritmus rekurzív megvalósítása.
        // Mivel rekurzív, ezért a visszalépésnek a "return null" felel meg.
        private Csúcs Keresés(Csúcs aktCsúcs, int részCélIndexe)
        {
            int mélység = aktCsúcs.GetMélység();
            // mélységi korlát vizsgálata
            if (korlát > 0 && mélység >= korlát) return null;
            // emlékezet használata kör kiszűréséhez
            Csúcs aktSzülő = null;
            if (emlékezetes) aktSzülő = aktCsúcs.GetSzülő();
            while (aktSzülő != null)
            {
                // Ellenőrzöm, hogy jártam-e ebben az állapotban. Ha igen, akkor visszalépés.
                if (aktCsúcs.Equals(aktSzülő)) return null;
                // Visszafelé haladás a szülői láncon.
                aktSzülő = aktSzülő.GetSzülő();
            }
            if (aktCsúcs.TerminálisCsúcsE())
            {
                // Megvan a megoldás, vissza kell adni a terminális csúcsot.
                return aktCsúcs;
            }

            for (int ti = 0; ti < aktCsúcs.TranzakciókSzáma(részCélIndexe); ti++)
            {
                Csúcs újCsúcs = new Csúcs(aktCsúcs);
                if (újCsúcs.Tranzakció(részCélIndexe, ti))
                {
                    // ha sikerült elérni a részcélt, akkor
                    // növelni kell a részCélIndexét
                    if (újCsúcs.RészCélE(részCélIndexe))
                    {
                        részCélIndexe++;
                    }
                    Csúcs terminális = Keresés(újCsúcs, részCélIndexe);
                    if (terminális != null)
                    {
                        return terminális;
                    }
                }
            }

            // Itt hívogatom az alapoperátorokat a szuper operátoron
            // keresztül. Ha valamelyik alkalmazható, akkor új csúcsot
            // készítek, és meghívom önmagamat rekurzívan.
            for (int i = 0; i < aktCsúcs.OperátorokSzáma(); i++)
            {
                // Elkészítem az új gyermek csúcsot.
                // Ez csak akkor lesz kész, ha alkalmazok rá egy alkalmazható operátort is.
                Csúcs újCsúcs = new Csúcs(aktCsúcs);
                // Kipróbálom az i.dik alapoperátort. Alkalmazható?
                if (újCsúcs.SzuperOperátor(i))
                {
                    // Ha igen, rekurzívan meghívni önmagam az új csúcsra.
                    // Ha nem null értéket ad vissza, akkor megvan a megoldás.
                    // Ha null értéket, akkor ki kell próbálni a következő alapoperátort.
                    Csúcs terminális = Keresés(újCsúcs, részCélIndexe);
                    if (terminális != null)
                    {
                        // Visszaadom a megoldást képviselő terminális csúcsot.
                        return terminális;
                    }
                    // Az else ágon kellene visszavonni az operátort.
                    // Erre akkor van szükség, ha az új gyermeket létrehozásában nem lenne klónozást.
                    // Mivel klónoztam, ezért ez a rész üres.
                }
            }
            // Ha kipróbáltam az összes operátort és egyik se vezetett megoldásra, akkor visszalépés.
            // A visszalépés hatására eggyel feljebb a következő alapoperátor kerül sorra.
            return null;
        }
    }
}