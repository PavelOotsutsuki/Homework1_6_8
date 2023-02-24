using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Homework1_6_8
{
    class Program
    {
        static void Main(string[] args)
        {
            Battle battle = new Battle();
            battle.Fight();
        }
    }

    abstract class Fighter
    {
        protected const int FullPercent = 100;

        protected int ArmorDefensePercent = 10;
        protected int Health;
        protected int MaxHealth;
        protected int Armor;
        protected int Damage;
        protected string ClassName;
        protected Random Random = new Random();

        public Fighter(int health, int armor, int damage, string name)
        {
            Health = health;
            Armor = armor;
            Damage = damage;
            Name = name;
            MaxHealth = Health;
        }

        public string Name { get; protected set; }

        public virtual void TakeDamage(int damage)
        {
            Health -= damage * (FullPercent - ArmorDefensePercent*Convert.ToInt32(Convert.ToBoolean(Armor))) / FullPercent;
            Armor -= damage * ArmorDefensePercent* Convert.ToInt32(Convert.ToBoolean(Armor)) / FullPercent;

            if (Armor<0)
            {
                Armor = 0;
            }
        }

        public void ShowInfo()
        {
            Console.WriteLine("Здоровье: " + Health);
            Console.WriteLine("Броня: " + Armor);
        }

        public virtual void ShowFullInfo()
        {
            Console.WriteLine("Имя: " + Name);
            Console.WriteLine("Класс: " + ClassName);
            Console.WriteLine("Здоровье: " + Health);
            Console.WriteLine("Атака: " + Damage);
            Console.WriteLine("Броня: " + Armor);
        }

        public bool IsAlive()
        {
            return Health > 0;
        }

        public abstract void MakeTurnMoves(Fighter defender);

        protected virtual void Attack(Fighter defender)
        {
            Console.WriteLine(Name + " атакует, пытаясь нанести " + Damage + " урона");
            defender.TakeDamage(Damage);
        }

        protected abstract void EndTurn();
    }

    class Mage: Fighter
    {
        private int _maxMana;
        private int _mana;
        private int _fireballDamage = 50;
        private int _manaToCastFireball = 4;

        public Mage(int health, int maxMana, int damage, string name, int armor=0): base(health,armor,damage,name)
        {
            _maxMana = maxMana;
            _mana = _maxMana;
            ClassName = "Маг";
        }

        public override void ShowFullInfo()
        {
            base.ShowFullInfo();
            Console.WriteLine("Мана: " + _mana);
        }

        public override void MakeTurnMoves(Fighter defender)
        {
            Console.WriteLine("1. Атака");
            Console.WriteLine("2. Использовать огненный шар");
            Console.WriteLine("3. Использовать Маначит");

            if (_mana >= 4)
            {
                CastFireball(defender);
            }
            else if (_mana <= _manaToCastFireball - 2 && _maxMana > _manaToCastFireball)
            {
                Attack(defender);
            }
            else
            {
                CastManachit();
            }

            EndTurn();
        }

        protected override void EndTurn()
        {
            _mana++;

            if (_mana > _maxMana)
            {
                _mana = _maxMana;
            }
        }

        private bool TryCastFireball(out int damage)
        {
            if (_mana>=_manaToCastFireball)
            {
                _mana -= _manaToCastFireball;
                damage = _fireballDamage;
                return true;
            }

            Console.WriteLine("Не хватает маны");
            damage = 0;
            return false;
        }

        private void CastFireball(Fighter defender)
        {
            if (TryCastFireball(out int damage))
            {
                Console.WriteLine(Name + " использует огненный шар, пытаясь нанести " + damage + " урона");
                defender.TakeDamage(damage);
            }
        }

        private void CastManachit()
        {
            _maxMana++;
            _mana = _maxMana;
            Console.WriteLine(Name + " использует Маначит, теперь у этого бойца максимум " + _maxMana + " маны");
        }
    }

    class Warrior:Fighter
    {
        private int _receivedArmorEndTurn = 2;

        public Warrior(int health, int damage, string name, int armor):base(health, armor, damage, name)
        {
            ClassName = "Воин";
        }

        public override void MakeTurnMoves(Fighter defender)
        {
            Console.WriteLine("1. Атака");
            Console.WriteLine("2. Использовать Удар броней");
            Console.WriteLine("3. Использовать Ярость брони");

            int command = Random.Next(0, 2);

            if (Armor == 0)
            {
                CastArmorRage();
            }
            else if (Armor > Damage)
            {
                CastShieldSlam(defender);
            }
            else if (command == 0)
            {
                Attack(defender);
            }
            else
            {
                CastArmorRage();
            }

            EndTurn();
        }

        protected override void EndTurn()
        {
            Armor += _receivedArmorEndTurn;
        }

        private void CastShieldSlam(Fighter defender)
        {
            Console.WriteLine(Name + " использует Удар броней и пытается нанести " + Armor + " урона");
            defender.TakeDamage(Armor);
        }

        private void CastArmorRage()
        {
            int receivedArmorWithoutArmor = 20;
            int receivedArmorWithArmor = 10;

            Console.Write(Name + " использует Ярость брони, получая ");

            if (Armor==0)
            {
                Armor += receivedArmorWithoutArmor;
                Console.Write(receivedArmorWithoutArmor);
            }
            else
            {
                Armor += receivedArmorWithArmor;
                Console.Write(receivedArmorWithArmor);
            }

            Console.WriteLine(" брони");
        }
    }

    class Priest:Fighter
    {
        private int _takenDamage = 0;
        private int _healingValueEndTurn = 2;
        private int _healingRagePercent = 10;
        private bool _isCastRakamosh;
        private int _rakamoshPercentToMaxHealth = 10;

        public Priest(int health, int damage, string name, int armor=0):base(health, armor, damage, name)
        {
            _isCastRakamosh = false;
            ClassName = "Жрец";
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            _takenDamage += damage * (FullPercent - ArmorDefensePercent * Convert.ToInt32(Convert.ToBoolean(Armor))) / FullPercent;
        }

        public override void MakeTurnMoves(Fighter defender)
        {
            Console.WriteLine("1. Атака");
            Console.WriteLine("2. Использовать Ярость лечения");
            Console.WriteLine("3. Использовать Ракамош");

            int command = Random.Next(0, 4);

            if (Health * 2 > MaxHealth)
            {
                Attack(defender);
            }
            else if (Health * 3 > MaxHealth)
            {
                if (command == 0)
                {
                    Attack(defender);
                }
                else
                {
                    CastHealingRage();
                }
            }
            else if (_isCastRakamosh == false)
            {
                CastRakamosh();
            }
            else
            {
                if (command == 1)
                {
                    Attack(defender);
                }
                else
                {
                    CastHealingRage();
                }
            }

            EndTurn();
        }

        private void CastHealingRage()
        {
            Damage += _takenDamage * _healingRagePercent/FullPercent;
            Console.WriteLine(Name + " применяет Ярость лечения, атака увеличилась до " + Damage);
        }

        private void CastRakamosh()
        {
            if (_isCastRakamosh==false)
            {
                MaxHealth += MaxHealth*_rakamoshPercentToMaxHealth / FullPercent;
                Health = MaxHealth;
                _isCastRakamosh = true;
                Console.WriteLine(Name + " применяет Ракамош, текущее здоровье - " + Health);
            }
            else
            {
                Console.WriteLine("Вы уже использовали это заклинание");
            }
        }

        protected override void EndTurn()
        {
            Health += _healingValueEndTurn;

            if (Health>MaxHealth)
            {
                Health = MaxHealth;
            }
        }
    }

    class Druid: Fighter
    {
        private int _healingValueEndTurn = 1;
        private int _damageValueEndTurn = 1;
        private int _receivedArmorEndTurn = 1;
        private int _criticalDamageChance = 10;
        private int _dodgeChance = 10;
        private int _criticalDamageValue = 2;
        private int _countWildGrowthMax;
        private int _countWildGrowthNow;

        public Druid(int health, int damage, string name, int armor) : base(health, armor, damage, name)
        {
            ClassName = "Друид";
            _countWildGrowthMax = Random.Next(2, 10);
            _countWildGrowthNow = 0;
        }

        public override void ShowFullInfo()
        {
            base.ShowFullInfo();
            Console.WriteLine("Шанс увернуться: " + _dodgeChance);
            Console.WriteLine("Шанс критического урона (x" + _criticalDamageValue + "): " + _criticalDamageChance);
        }

        public override void TakeDamage(int damage)
        {
            int dodgeChance = Random.Next(1, FullPercent + 1);

            if (dodgeChance >= _dodgeChance)
            {
                base.TakeDamage(damage);
            }
            else
            {
                Console.WriteLine("Уворот!");
            }
        }

        public override void MakeTurnMoves(Fighter defender)
        {
            Console.WriteLine("1. Атака");
            Console.WriteLine("2. Использовать Буйный рост");

            if (_countWildGrowthNow < _countWildGrowthMax)
            {
                _countWildGrowthNow++;
                CastWildGrowth();
            }
            else
            {
                Attack(defender);
            }

            EndTurn();
        }

        private void CastWildGrowth()
        {
            _healingValueEndTurn++;
            _damageValueEndTurn++;
            _receivedArmorEndTurn++;
            Console.WriteLine(Name + " использует Буйный рост. Теперь в конце хода вы получаете " + _healingValueEndTurn + " хп, " + _receivedArmorEndTurn + " брони и прибавку к атаке: " + _damageValueEndTurn);
        }

        protected override void Attack(Fighter defender)
        {
            int damage;
            int criticalDamageValue;
            int criticalDamageChance = Random.Next(1, FullPercent + 1);

            if (criticalDamageChance >= _criticalDamageChance)
            {
                criticalDamageValue = 1;
            }
            else
            {
                criticalDamageValue = _criticalDamageValue;
            }

            damage = Damage * criticalDamageValue;
            defender.TakeDamage(damage);
            Console.WriteLine(Name + " атакует, пытаясь нанести " + damage + " урона");
        }

        protected override void EndTurn()
        {
            Health += _healingValueEndTurn;

            if (Health > MaxHealth)
            {
                Health = MaxHealth;
            }

            Damage += _damageValueEndTurn;
            Armor += _receivedArmorEndTurn;
        }
    }

    class Rogue:Fighter
    {
        private int _dodgeChance = 20;
        private int _receivedDodgeChance = 20;
        private int _criticalDamageChance = 20;
        private int _receivedCriticalDamageChance = 20;
        private int _criticalDamageValue = 2;

        public Rogue(int health, int damage, string name, int armor=0) : base(health, armor, damage, name)
        {
            ClassName = "Разбойник";
        }

        public override void ShowFullInfo()
        {
            base.ShowFullInfo();
            Console.WriteLine("Шанс увернуться: " + _dodgeChance);
            Console.WriteLine("Шанс критического урона (x" + _criticalDamageValue + "): " + _criticalDamageChance);
        }

        private void CastShadow()
        {
            _dodgeChance += (FullPercent - _dodgeChance) * _receivedDodgeChance / FullPercent;
            Console.WriteLine(Name + " использовал Тень. Теперь шанс уклониться от атаки " + _dodgeChance + "%");
        }

        private void CastSneakyTrick()
        {
            _criticalDamageChance += (FullPercent - _criticalDamageChance) * _receivedCriticalDamageChance / FullPercent;
            Console.WriteLine(Name + " использовал Подлый трюк. Теперь шанс нанести критический урон " + _criticalDamageChance + "%");
        }

        public override void TakeDamage(int damage)
        {
            int dodgeChance = Random.Next(1, FullPercent + 1);

            if (dodgeChance >= _dodgeChance)
            {
                base.TakeDamage(damage);
            }
            else
            {
                Console.WriteLine("Уворот!");
            }
        }

        protected override void Attack(Fighter defender)
        {
            int damage;
            int criticalDamageValue;
            int criticalDamageChance = Random.Next(1, FullPercent + 1);

            if (criticalDamageChance >= _criticalDamageChance)
            {
                criticalDamageValue = 1;
            }
            else
            {
                criticalDamageValue = _criticalDamageValue;
            }

            damage = Damage * criticalDamageValue;
            defender.TakeDamage(damage);
            Console.WriteLine(Name + " атакует, пытаясь нанести " + damage + " урона");
        }

        protected override void EndTurn()
        {
            Attack(defender);
        }

        public override void MakeTurnMoves(Fighter defender)
        {
            Console.WriteLine("1. Атака");
            Console.WriteLine("2. Использовать Тень");
            Console.WriteLine("3. Использовать Подлый трюк");

            int command = Random.Next(0, 5);

            if (command==0)
            {
                CastShadow();
            }
            else if (command==1)
            {
                CastSneakyTrick();
            }
            else
            {
                Attack(defender);
            }

            EndTurn();
        }
    }

    class Battle
    {
        const int FirstFighterNumber = 1;
        const int SecondFighterNumber = 2;

        private List<Fighter> _fighters = new List<Fighter>();
        private Fighter _fighter1;
        private Fighter _fighter2;

        public Battle ()
        {
            _fighters.Add(new Mage(800, 3, 10, "Джайна"));
            _fighters.Add(new Warrior(1000, 15, "Гаррош", 10));
            _fighters.Add(new Priest(600, 10, "Андуин"));
            _fighters.Add(new Druid(400, 15, "Малфурион", 5));
            _fighters.Add(new Rogue(400, 20, "Валира"));
        }

        public void Fight()
        {
            if (TryGetFighter(out Fighter fighter1, FirstFighterNumber) && TryGetFighter(out Fighter fighter2, SecondFighterNumber))
            {
                _fighter1 = fighter1;
                _fighter2 = fighter2;

                while (_fighter1.IsAlive() && _fighter2.IsAlive())
                {
                    Console.Clear();
                    _fighter1.MakeTurnMoves(_fighter2);
                    _fighter2.MakeTurnMoves(_fighter1);
                    ShowFighterInfo(_fighter1, FirstFighterNumber);
                    ShowFighterInfo(_fighter2, SecondFighterNumber);
                    Console.WriteLine("------------------------------");
                    Console.ReadKey();
                }

                if (_fighter1.IsAlive())
                {
                    Console.WriteLine("Победитель - " + _fighter1.Name);
                }
                else if (_fighter2.IsAlive())
                {
                    Console.WriteLine("Победитель - " + _fighter2.Name);
                }
                else
                {
                    Console.Write("Ничья!");
                }
            }
        }

        private void ShowFighterInfo(Fighter fighter, int fighterNumber)
        {
            Console.WriteLine("------------------------------");
            Console.WriteLine("Игрок " + fighterNumber + ": ");
            fighter.ShowInfo();
            Console.WriteLine();
        }

        private bool TryGetFighter(out Fighter fighter, int addedFighterInBattleNumber)
        {
            Console.Clear();

            for (int i = 0; i < _fighters.Count; i++)
            {
                Console.WriteLine("Боец " + (i + 1) + ": ");
                _fighters[i].ShowFullInfo();
                Console.WriteLine();
            }

            Console.Write("Выберите номер " + addedFighterInBattleNumber + "-го бойца: ");

            if (int.TryParse(Console.ReadLine(), out int fighterNumber))
            {
                if (fighterNumber >= 1 && fighterNumber <= _fighters.Count)
                {
                    fighter=_fighters[fighterNumber - 1];
                    _fighters.Remove(_fighters[fighterNumber - 1]);
                    return true;
                }
                else
                {
                    Console.WriteLine("Бойца под таким номером не существует");
                }
            }
            else
            {
                Console.WriteLine("Введено некорректное значение");
            }

            fighter = null;
            return false;
        }
    }
}
