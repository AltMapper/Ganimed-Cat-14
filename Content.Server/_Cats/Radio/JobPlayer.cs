using Content.Server.Access.Systems;
using Robust.Shared.Utility;

namespace Content.Server._Cats.Radio;

/// <summary>
/// Данный класс отвечает за отображения должностей в канале рации. То есть, если ошибка в Content.Radi.EnitySystem - вам сюда. Так же в самом оригинальном коде рации есть способ быстро востановить его оригинальную работу без моего класса
/// </summary>
public sealed class JobPlayer : EntitySystem
{
    // Для работы с id картой
    [Dependency] private readonly IdCardSystem _idCardSystem = default!;

    // Теперь словарь не будет создаваться каждый раз при вызове метода GetColorPlayer
    // // Тут хранятся цвета для должностей. У должности все буквы в нижнем регистре, поэтому заглавными писать ничего не надо. Избегайте повторных ключей
    public static readonly Dictionary<string, string> JobColors = new()
    {
        // Неизвестно
        { "неизвестно", "lime" },
        // Синтетики
        { "борг", "white" },
        { "юнит", "white" },
        { "ии", "white" },
        { "искусственный интеллект", "white" },
        // Кеп
        { "капитан", "yellow" },
        { "врио капитан", "yellow" },
        //ЦК
        { "цк", "yellow" },
        { "центральное командование", "yellow" },
        // Главы
        { "адъютант", "#3065e8" },
        { "старший инженер", "#3065e8" },
        { "си", "#3065e8" },
        { "врио старший инженер", "#3065e8" },
        { "врио си", "#3065e8" },
        { "научный руководитель", "#3065e8" },
        { "нр", "#3065e8" },
        { "врио нр", "#3065e8" },
        { "врио научный руководитель", "#3065e8" },
        { "глава персонала", "#3065e8" },
        { "врио глава персонала", "#3065e8" },
        { "врио гп", "#3065e8" },
        { "гп", "#3065e8" },
        { "глава службы безопасности", "#3065e8" },
        { "врио глава службы безопасности", "#3065e8" },
        { "врио гсб", "#3065e8" },
        { "гсб", "#3065e8" },
        { "квартирмейстер", "#3065e8" },
        { "врио квартирмейстер", "#3065e8" },
        { "врио км", "#3065e8" },
        { "км", "#3065e8" },
        { "главный врач", "#3065e8" },
        { "врио главный врач", "#3065e8" },
        { "врио гв", "#3065e8" },
        { "гв", "#3065e8" },
        //ЦК
        { "пцк", "yellow" },
        { "представитель цк", "yellow" },
        { "представитель центком", "yellow" },
        { "представитель центрального командования", "yellow" },
        //СБ
        { "офицер сб", "#ff2727" },
        { "кадет сб", "#ff2727" },
        { "смотритель", "#ff2727" },
        { "детектив", "#ff2727" },
        { "старший офицер", "#ff2727" },
        { "со", "#ff2727" },
        { "бригмед", "#ff2727" },
        { "бригмедик", "#ff2727" },
        { "бм", "#ff2727" },
        //Инжи
        { "инженер станции", "orange" },
        { "инженер", "orange" },
        { "атмосферный техник", "orange" },
        { "атмос", "orange" },
        { "ведущий инженер", "orange" },
        { "ви", "orange" },
        { "технический ассистент", "orange" },
        //РНД
        { "научный ассистент", "mediumpurple" },
        { "учёный", "mediumpurple" },
        { "ученый", "mediumpurple" },
        { "робототехник", "mediumpurple" },
        { "старший научный сотрудник", "mediumpurple" },
        { "снс", "mediumpurple" },
        //Сервис
        { "сервисный работник", "green" },
        { "зоотехник", "green" },
        { "репортёр", "green" },
        { "репортер", "green" },
        { "пассажир", "lime" },
        { "музыкант", "#aaeeaf" },
        { "мим", "#aaeeaf" },
        { "библиотекарь", "#aaeeaf" },
        { "юрист", "#aaeeaf" },
        { "уборщик", "#aaeeaf" },
        { "клоун", "#aaeeaf" },
        { "шеф-повар", "#aaeeaf" },
        { "священник", "#aaeeaf" },
        { "боксёр", "#aaeeaf" },
        { "боксер", "#aaeeaf" },
        { "ботаник", "#aaeeaf" },
        { "бармен", "#aaeeaf" },
        //Карго
        { "грузчик", "#7b3f00" },
        { "утилизатор", "#7b3f00" },
        //Мед
        { "интерн", "skyblue" },
        { "врач", "skyblue" },
        { "доктор", "skyblue" },
        { "химик", "skyblue" },
        { "парамедик", "skyblue" },
        { "хирург", "skyblue" },
        { "патологоанатом", "skyblue" },
        { "психолог", "skyblue" },
        { "ведущий врач", "skyblue" },
        //Юр деп
        { "агент внутренних дел", "pink" },
        { "авд", "pink" },
        { "магистрат", "pink" },
        // ДСО - департамент спецаилбных операций
        { "офицер специальных операций", "#C0C0C0" },
        { "офицер спец операций", "#C0C0C0" },
        { "осо", "#C0C0C0" },
        { "дсо", "#C0C0C0" },
        { "эскадрон смерти", "#C0C0C0" },
        { "эс", "#C0C0C0" },
        { "уборщик обр", "#C0C0C0" },
        { "обр", "#C0C0C0" },
        { "инженер обр", "#C0C0C0" },
        { "служба безопасности обр", "#C0C0C0" },
        { "медик обр", "#C0C0C0" },
        { "лидер обр", "#C0C0C0" },
        { "лидер рхбзз", "#C0C0C0" },
        { "уборщик рхбзз", "#C0C0C0" },
        { "рхбзз", "#C0C0C0" },
        { "инженер рхбзз", "#C0C0C0" },
        { "служба безопасности рхбзз", "#C0C0C0" },
        { "медик рхбзз", "#C0C0C0" },
        //ОСЩ
        { "офицер синий щит", "#00BFFF" },
        { "офицер \"синий щит\"", "#00BFFF" },
        { "офицер «синий щит»", "#00BFFF" },
        { "осщ", "#00BFFF" },
        { "синий щит", "#00BFFF" },
        { "офицер мостика", "#00BFFF" },
        { "офик мостика", "#00BFFF" },
        // Прочее
        { "заключённый", "#BDB76B" },
        { "беглец", "#BDB76B" },
        { "лидер РХБЗЗ", "#C0C0C0" },
        { "оперативник РХБЗЗ", "#BDB76B" },
        { "Omicron - 3", "#C0C0C0" },
        { "Сотрудник Мобильной Группы", "#C0C0C0" },
        { "Сотрудник Исследовательской Группы", "#C0C0C0" },
        { "Агент Красного Ордена", "#BDB76B" },
        { "Представитель ОПЗ", "#C0C0C0" },
        { "Офицер ОПЗ", "#C0C0C0" },
        { "Epsilon-11", "#C0C0C0" },
        { "Alpha-1", "#C0C0C0" },
        { "Tay-39", "#C0C0C0" },
        { "Omega-0", "#C0C0C0" },
        { "Офицер Специальный Операций", "#C0C0C0" },
        { "Sierra - 6", "#C0C0C0" },
        { "Theta-9", "#C0C0C0" },
        { "Войска РХБЗЗ Sierra-6", "#C0C0C0" },
        { "Рядовой", "#C0C0C0" },
        { "Капрал", "#C0C0C0" },
        { "Сержант", "#C0C0C0" },
        { "Младший унтер-офицер", "#C0C0C0" },
        { "Унтер-офицер", "#C0C0C0" },
        { "Штаб-офицер", "#C0C0C0" },
        { "Лейтенант", "#C0C0C0" },
        { "Старший лейтенант", "#C0C0C0" },
        { "Майор", "#C0C0C0" },
        { "Подполковник", "#C0C0C0" },
        { "Полковник", "#C0C0C0" },
        { "Генерал-Майор", "#C0C0C0" },
        { "Генерал-Лейтенант", "#C0C0C0" },
        { "Генерал-Полковник", "#C0C0C0" },
        { "Горничная", "#FDDDE6"}
    };


	/// <summary>
    /// Для EnityUid ищется карта в руках или в пда. Если карта нашлась, мы смотрим какая там работа и достаём её
    /// </summary>
    /// <param name="uid"> uid игрока, у которого мы ищем карту</param>
    /// <returns></returns>
    public string? GetJobPlayer(EntityUid uid)
    {
        // Проверяем нашлась ли карта и какое id у карты
        if (_idCardSystem.TryFindIdCard(uid, out var id))
        {
            // Мы сохраняем работу
            string? playerJob = id.Comp.LocalizedJobTitle;
            // Если работа нашлась верно, мы начинаем основной процесс
            if (playerJob != null)
            {
                // Перевод для некоторых ролей, которые у нас не переведены
                if (playerJob == "Central Commander")
                {
                    playerJob = $"Центральное Командование";
                }

                // Перевод 2
                if (playerJob == "Centcom Quarantine Officer")
                {
                    playerJob = $"Офицер Специальных Операций";
                }

                // Делаем начало должности с заглавной буквы и сохраняем в playerJob
                playerJob = char.ToUpper(playerJob[0]) + playerJob.Substring(1);
                // Убрав лишние пробелы, передаём полученное значение
                return playerJob.Trim();

            }


        }
        // Если работы нет, то возвращается должность "Неизвестно"
        return "Неизвестно";
    }

    /// <summary>
    /// Метод, который отвечает за подбор цвета для должности. Используется словарь, который работает по О(1), что быстрее if и подобного
    /// </summary>
    /// <param name="jobPlayer"> Нужно получить из метода GetJobPlayer. Или же вставляйте сюда свою string? работу, но нужно привести её к нижниму регистру для словаря. Метод ваш_стринг.ToLower()</param>
    /// <returns></returns>
    public string? GetColorPlayer(string? jobPlayer)
    {
        // Проверка. Работаем только тогда, когда работа была определена успешно
        if (jobPlayer != null)
        {
            // Преобразуем jobPlayer к нижнему регистру для поиска в словаре
            string normalizedJob = jobPlayer.ToLower();

            // Ищем цвет по нормализованному значению должности
            string color = JobColors.TryGetValue(normalizedJob, out var jobColor) ? jobColor : "lime";
            return color;

        }
        // На всякий случай проверка ещё раз
        return null;
    }

    /// <summary>
    /// Данный метод, используя два прошлых метода, формирует уже необходимый name, который и будет отображён в рации. На самом деле, его можно применять и к ChatSystem.cs
    /// </summary>
    /// <param name="uid"> id отправителя. В оригинальном коде рации messageSource</param>
    /// <param name="name"> string?. Это по сути то, что будет указано в отправителе. Раньше там писалось просто имя</param>
    /// <returns></returns>
    public string CompletedJobAndPlayer(EntityUid uid,  string? name)
    {
        // Активируем метод для определения должности
        string? nameJobPlayer = GetJobPlayer(uid);
        // Если должность определена, добавляем
        if (nameJobPlayer != null)
        {
            // Добавляем должность. Важно это сделать перед FormattedMessage
            name = $"[{nameJobPlayer}] {name}";
        }

        // Это необходимость, которая присутствует в оригинальном коде рации. До неё color и болд не будут работать. А после неё уже не добавить текст, не будет отображаться в рации
        if (name != null)
        {
            name = FormattedMessage.EscapeText(name);
        }

        // Определяем какой цвет должен быть у данной должности
        string? nameColorPlayer = GetColorPlayer(nameJobPlayer);

        // Если всё сработало правильно, будет работать этот код. Я даже не могу придумать случаев, когда будет работать не это условие, а return вконце метода
        if ((nameColorPlayer != null) && (nameJobPlayer != null))
        {
            // Тут идёт формирование как раз необходимого имени с должностью и цветом
            string? nameEnd = $"[bold][color={nameColorPlayer}][{nameJobPlayer}] {name}[/bold][/color]";
            // Возвращаем
            return nameEnd;
        }

        // Если произошло второе пришествие, мы всё равно отправляем [Неизвестный] Имя (цвет лайма). Всё равноо C# не позволит мне удалить этот return
        return $"[bold][color=lime][Неизвестно] {Name(uid)}[/bold][/color]";

    }


}
