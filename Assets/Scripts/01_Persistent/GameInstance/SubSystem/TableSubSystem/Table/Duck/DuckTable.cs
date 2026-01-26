using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum EDuckType
{ 
    Player = 0 ,

    // Enemy
    Farmer,     // 농부
    Mercenary,  // 용병
    Boxer,      // 복서
      
    // NPC
    Jeff = 100,
     
    // etc 
    Anyone,

    End,
}
public enum EDuckRelation
{
    Friendly, 
    Hostile,
    Neutral,
}

public class DuckTable : MonoBehaviour
{
    private Dictionary<EDuckType, DuckDefaultInfo>  hashDuckStatTable = new ();
    private Dictionary<EDuckType, DuckDropItem>     hashDuckDropTable = new ();
    private Dictionary<EDuckType, string>           hashDuckName = new ();
    private Dictionary<(EDuckType, EDuckType), EDuckRelation> duckRelationTable = new();
     
    ////////
    private Dictionary<EAdornBodyType, string> hashAdornBodyName = new();
    private Dictionary<EAdornHairType, string> hashAdornHairName = new();
    private Dictionary<EAdornEyesBowType, string> hashAdornEyesBowName = new();
    private Dictionary<EAdornEyesType, string> hashAdornEyesName = new();
    private Dictionary<EAdornBeekType, string> hashAdornBeekName = new();
    private Dictionary<EAdornHandType, string> hashAdornHandName = new();
    private Dictionary<EAdornFootType, string> hashAdornFootName = new();

    private void Awake()
    {
        // LoadData();
        // MakeDuckName();
        // MakeDuckRelation();
        // MakeAdornNameTable();
    } 
     
    public void Init()
    { 
        LoadData();
        MakeDuckName();
        MakeDuckRelation();
        MakeAdornNameTable();
    }
       
    public DuckDefaultInfo GetDuckDefaultInfo(EDuckType type)
    {
        return hashDuckStatTable[type];
    } 
    public DuckDropItem GetDuckDropItem(EDuckType type)
    {
        return hashDuckDropTable[type];
    }
    public EDuckRelation GetDuckRelation(EDuckType _a, EDuckType _b)
    {
        if (_a == _b)
            return EDuckRelation.Friendly;

        if (duckRelationTable.TryGetValue((_a, _b), out var rel))
            return rel;

        return EDuckRelation.Neutral;
    }

    public string GetDuckName(EDuckType type)
    {
        if (hashDuckName.TryGetValue(type, out var name))
            return name;

        return type.ToString(); // fallback
    } 
    public string GetAdornName(EAdornBodyType type)     => hashAdornBodyName.TryGetValue(type, out var name) ? name : type.ToString();
    public string GetAdornName(EAdornHairType type)     => hashAdornHairName.TryGetValue(type, out var name) ? name : type.ToString();
    public string GetAdornName(EAdornEyesType type)     => hashAdornEyesName.TryGetValue(type, out var name) ? name : type.ToString();
    public string GetAdornName(EAdornEyesBowType type)  => hashAdornEyesBowName.TryGetValue(type, out var name) ? name : type.ToString();
    public string GetAdornName(EAdornBeekType type)     => hashAdornBeekName.TryGetValue(type, out var name) ? name : type.ToString();
    public string GetAdornName(EAdornHandType type)     => hashAdornHandName.TryGetValue(type, out var name) ? name : type.ToString();
    public string GetAdornName(EAdornFootType type)     => hashAdornFootName.TryGetValue(type, out var name) ? name : type.ToString();

     
    private void LoadData()
    {
        // Duck 별 기본 스텟정보
        var statDatas = Resources.LoadAll<DuckDefaultInfo>("Data/Duck/DefaultInfo");
        foreach (var data in statDatas)
        {
            hashDuckStatTable[data.duckType] = data;
        } 

        // Duck 별 기본 드랍 아이템정보
        var dropDatas = Resources.LoadAll<DuckDropItem>("Data/Duck/DropItem");
        foreach (var data in dropDatas)
        {
            hashDuckDropTable[data.duckType] = data;
        } 
    }

    private void MakeDuckName()
    {
        hashDuckName.Clear();

        hashDuckName[EDuckType.Player] = "플레이어";

        // Enemy
        hashDuckName[EDuckType.Farmer] = "농부";
        hashDuckName[EDuckType.Mercenary] = "용병";
        hashDuckName[EDuckType.Boxer] = "복서";
         
        // NPC 
        hashDuckName[EDuckType.Jeff] = "제프";

        // Ani  
        hashDuckName[EDuckType.Anyone] = "적 아무나";
    }
    private void MakeDuckRelation()
    {
        duckRelationTable.Clear();

        // Player vs Farmer, Mercenary, Boxer
        duckRelationTable[(EDuckType.Player, EDuckType.Farmer)] = EDuckRelation.Hostile;
        duckRelationTable[(EDuckType.Player, EDuckType.Mercenary)] = EDuckRelation.Hostile;
        duckRelationTable[(EDuckType.Player, EDuckType.Boxer)] = EDuckRelation.Hostile;

        // Farmer vs Player, Boxer, Mercenary
        duckRelationTable[(EDuckType.Farmer, EDuckType.Player)] = EDuckRelation.Hostile;
        duckRelationTable[(EDuckType.Farmer, EDuckType.Boxer)] = EDuckRelation.Hostile;
        duckRelationTable[(EDuckType.Farmer, EDuckType.Mercenary)] = EDuckRelation.Hostile;

        // Boxer vs Mercenary, Farmer, Player
        duckRelationTable[(EDuckType.Boxer, EDuckType.Mercenary)] = EDuckRelation.Hostile;
        duckRelationTable[(EDuckType.Boxer, EDuckType.Farmer)] = EDuckRelation.Hostile;
        duckRelationTable[(EDuckType.Boxer, EDuckType.Player)] = EDuckRelation.Hostile;

        // Mercenary vs Farmer, Boxer, Player
        duckRelationTable[(EDuckType.Mercenary, EDuckType.Farmer)] = EDuckRelation.Hostile;
        duckRelationTable[(EDuckType.Mercenary, EDuckType.Boxer)] = EDuckRelation.Hostile;
        duckRelationTable[(EDuckType.Mercenary, EDuckType.Player)] = EDuckRelation.Hostile;
    }
    private void MakeAdornNameTable()
    {
        // Body
        hashAdornBodyName[EAdornBodyType.Default] = "기본 몸";
        hashAdornBodyName[EAdornBodyType.End] = "없음";
         
        // Hair
        hashAdornHairName[EAdornHairType.Low] = "로우 헤어";
        hashAdornHairName[EAdornHairType.PingBa] = "핑바 헤어";
        hashAdornHairName[EAdornHairType.Gourd] = "바가지 헤어";
        hashAdornHairName[EAdornHairType.Parting] = "가르마 헤어";
        hashAdornHairName[EAdornHairType.End] = "없음"; 
         
        // EyesBow
        hashAdornEyesBowName[EAdornEyesBowType.RoundBow] = "둥근 눈썹";
        hashAdornEyesBowName[EAdornEyesBowType.Unibrow] = "일자 눈썹";
        hashAdornEyesBowName[EAdornEyesBowType.JJangu] = "짱구 눈썹";
        hashAdornEyesBowName[EAdornEyesBowType.End] = "없음";
         
        // Eyes
        hashAdornEyesName[EAdornEyesType.Default] = "기본 눈";
        hashAdornEyesName[EAdornEyesType.Button] = "단추 눈";
        hashAdornEyesName[EAdornEyesType.Heart] = "하트 눈"; 
        hashAdornEyesName[EAdornEyesType.BottleCup] = "병뚜껑 눈";
        hashAdornEyesName[EAdornEyesType.KeyCap] = "키캡 눈";
        hashAdornEyesName[EAdornEyesType.Shining] = "반짝 눈";
        hashAdornEyesName[EAdornEyesType.End] = "없음";
         
        // Beek
        hashAdornBeekName[EAdornBeekType.Default] = "기본 부리";
        hashAdornBeekName[EAdornBeekType.ShoeBill] = "신발 부리";
        hashAdornBeekName[EAdornBeekType.StraightShort] = "짧은 부리";
        hashAdornBeekName[EAdornBeekType.Toucan] = "투칸 부리";
        hashAdornBeekName[EAdornBeekType.BlackFacedSpoonbillShort] = "저어새 부리";
        hashAdornBeekName[EAdornBeekType.Pacifier] = "쪽쪽이 부리"; 
        hashAdornBeekName[EAdornBeekType.End] = "없음";
        
        // Hand
        hashAdornHandName[EAdornHandType.Default] = "기본 팔";
        hashAdornHandName[EAdornHandType.HandBoxing] = "복싱 장갑";
        hashAdornHandName[EAdornHandType.Hand] = "손";
        hashAdornHandName[EAdornHandType.End] = "없음";
          
        // Foot
        hashAdornFootName[EAdornFootType.Default] = "기본 다리";
        hashAdornFootName[EAdornFootType.Foot] = "신발";
        hashAdornFootName[EAdornFootType.End] = "없음";
    }
}
