using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
        protected const int WithoutDamage = 0;

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

        protected virtual int Attack()
        {
            Console.WriteLine(Name + " атакует, пытаясь нанести " + Damage + " урона");
            return Damage;
        }

        public bool Leave()
        {
            if (Health > 0)
            {
                return true;
            }

            return false;
        }

        abstract protected int EndTurn();
        abstract public int GetAttackOfTurn();
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

        private bool TryCastFireball(out int damage)
        {
            if (_mana>=_manaToCastFireball)
            {
                _mana -= _manaToCastFireball;
                damage = _fireballDamage;
                return true;
            }

            damage = 0;
            return false;
        }

        private int CastFireball()
        {
            TryCastFireball(out int damage);
            Console.WriteLine(Name + " использует огненный шар, пытаясь нанести " + damage + " урона");

            return damage;
        }

        private void CastManachit()
        {
            _maxMana++;
            _mana = _maxMana;
            Console.WriteLine(Name + " использует Маначит, теперь у этого бойца максимум " + _maxMana + " маны");
        }

        protected override int EndTurn()
        {
            _mana++;

            if(_mana>_maxMana)
            {
                _mana = _maxMana;
            }

            return WithoutDamage;
        }

        public override int GetAttackOfTurn()
        {
            int dealtDamage = 0;

            Console.WriteLine("1. Атака");
            Console.WriteLine("2. Использовать огненный шар");
            Console.WriteLine("3. Использовать Маначит");

            if(_mana>=4)
            {
                dealtDamage += CastFireball();
            }
            else if(_mana<= _manaToCastFireball-2 && _maxMana>_manaToCastFireball)
            {
                dealtDamage += Attack();
            }
            else
            {
                CastManachit();
            }

            dealtDamage+=EndTurn();

            return dealtDamage;
        }
    }

    class Warrior:Fighter
    {
        private int _receivedArmorEndTurn = 2;

        public Warrior(int health, int damage, string name, int armor):base(health, armor, damage, name)
        {
            ClassName = "Воин";
        }

        private int CastShieldSlam()
        {
            Console.WriteLine(Name + " использует Удар броней и пытается нанести " + Armor + " урона");
            return Armor;
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

        protected override int EndTurn()
        {
            Armor += _receivedArmorEndTurn;
            return WithoutDamage;
        }
        
        public override int GetAttackOfTurn()
        {
            int dealtDamage = 0;
            
            Console.WriteLine("1. Атака");
            Console.WriteLine("2. Использовать Удар броней");
            Console.WriteLine("3. Использовать Ярость брони");

            int command = Random.Next(0, 2);

            if (Armor==0)
            {
                CastArmorRage();
            }
            else if (Armor>Damage)
            {
                dealtDamage+=CastShieldSlam();
            }
            else if(command==0)
            {
                dealtDamage += Attack();
            }
            else
            {
                CastArmorRage();
            }

            dealtDamage+=EndTurn();

            return dealtDamage;
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

        private void CastHealingRage()
        {
            Damage += _takenDamage * _healingRagePercent/FullPercent;
            Console.WriteLine(Name + " применяет Ярость лечения, атака увеличилась до " + Damage);
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            _takenDamage+= damage * (FullPercent - ArmorDefensePercent * Convert.ToInt32(Convert.ToBoolean(Armor))) / FullPercent;
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

        protected override int EndTurn()
        {
            Health += _healingValueEndTurn;

            if (Health>MaxHealth)
            {
                Health = MaxHealth;
            }

            return WithoutDamage;
        }

        public override int GetAttackOfTurn()
        {
            int dealtDamage = 0;

            Console.WriteLine("1. Атака");
            Console.WriteLine("2. Использовать Ярость лечения");
            Console.WriteLine("3. Использовать Ракамош");

            int command = Random.Next(0, 4);

            if (Health*2>MaxHealth)
            {
                dealtDamage+=Attack();
            }
            else if (Health * 3 > MaxHealth)
            {
                if (command==0)
                {
                    dealtDamage += Attack();
                }
                else
                {
                    CastHealingRage();
                }
            }
            else if (_isCastRakamosh==false)
            {
                CastRakamosh();
            }
            else
            {
                if (command == 1)
                {
                    dealtDamage += Attack();
                }
                else
                {
                    CastHealingRage();
                }
            }

            dealtDamage+=EndTurn();

            return dealtDamage;
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

        private void CastWildGrowth()
        {
            _healingValueEndTurn++;
            _damageValueEndTurn++;
            _receivedArmorEndTurn++;
            Console.WriteLine(Name + " использует Буйный рост. Теперь в конце хода вы получаете " + _healingValueEndTurn + " хп, " + _receivedArmorEndTurn + " брони и прибавку к атаке: " + _damageValueEndTurn);
        }

        protected override int Attack()
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
            Console.WriteLine(Name + " атакует, пытаясь нанести " + damage + " урона");
            return damage;
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

        protected override int EndTurn()
        {
            Health += _healingValueEndTurn;

            if (Health > MaxHealth)
            {
                Health = MaxHealth;
            }

            Damage += _damageValueEndTurn;
            Armor += _receivedArmorEndTurn;
            return WithoutDamage;
        }

        public override int GetAttackOfTurn()
        {
            int dealtDamage = 0;

            Console.WriteLine("1. Атака");
            Console.WriteLine("2. Использовать Буйный рост");

            if (_countWildGrowthNow< _countWildGrowthMax)
            {
                _countWildGrowthNow++;
                CastWildGrowth();
            }
            else
            {
                dealtDamage += Attack();
            }

            dealtDamage+=EndTurn();

            return dealtDamage;
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

        protected override int Attack()
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
            Console.WriteLine(Name + " атакует, пытаясь нанести " + damage + " урона");
            return damage;
        }

        protected override int EndTurn()
        {
            return Attack();
        }

        public override int GetAttackOfTurn()
        {
            int dealtDamage = 0;

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
                dealtDamage+=Attack();
            }

            dealtDamage += EndTurn();

            return dealtDamage;
        }
    }

    class Battle
    {
        private List<Fighter> _fighters = new List<Fighter>();
        private List<Fighter> _fightersInBattle=new List<Fighter>();

        public Battle ()
        {
            _fighters.Add(new Mage(500, 3, 10, "Джайна"));
            _fighters.Add(new Warrior(500, 15, "Гаррош", 10));
            _fighters.Add(new Priest(500, 10, "Андуин"));
            _fighters.Add(new Druid(500, 15, "Малфурион", 5));
            _fighters.Add(new Rogue(400, 20, "Валира"));
        }

        public void Fight()
        {
            if (TryGetFighter() && TryGetFighter())
            {
                while (_fightersInBattle[0].Leave() && _fightersInBattle[1].Leave())
                {
                    Console.Clear();
                    _fightersInBattle[1].TakeDamage(_fightersInBattle[0].GetAttackOfTurn());
                    _fightersInBattle[0].TakeDamage(_fightersInBattle[1].GetAttackOfTurn());
                    Console.WriteLine("------------------------------");
                    Console.WriteLine("Игрок 1: ");
                    _fightersInBattle[0].ShowInfo();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("Игрок 2: ");
                    _fightersInBattle[1].ShowInfo();
                    Console.WriteLine("------------------------------");
                    Console.ReadLine();
                }

                if (_fightersInBattle[0].Leave())
                {
                    Console.WriteLine("Победитель - " + _fightersInBattle[0].Name);
                }
                else if (_fightersInBattle[1].Leave())
                {
                    Console.WriteLine("Победитель - " + _fightersInBattle[1].Name);
                }
                else
                {
                    Console.Write("Ничья!");
                }
            }
        }

        private bool TryGetFighter()
        {
            Console.Clear();

            for (int i = 0; i < _fighters.Count; i++)
            {
                Console.WriteLine("Боец " + (i + 1) + ": ");
                _fighters[i].ShowFullInfo();
                Console.WriteLine();
            }

            Console.Write("Выберите номер " + (_fightersInBattle.Count+1) + "-го бойца: ");

            if (int.TryParse(Console.ReadLine(), out int fighterNumber))
            {
                if (fighterNumber >= 1 && fighterNumber <= _fighters.Count)
                {
                    _fightersInBattle.Add(_fighters[fighterNumber - 1]);
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

            return false;
        }
    }
}
