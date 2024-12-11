using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Address
{
    public enum GarAddressLevel
    {
        [AddressLevel("Субъект РФ")]
        Region = 1,
        [AddressLevel("Административный район")]
        AdministrativeArea = 2,
        [AddressLevel("Муниципальный район")]
        MunicipalArea = 3,
        [AddressLevel("Сельское/городское поселение")]
        RuralUrbanSettlement = 4,
        [AddressLevel("Город")]
        City = 5,
        [AddressLevel("Населенный пункт")]
        Locality = 6,
        [AddressLevel("Элемент планировочной структуры")]
        ElementOfPlanningStructure = 7,
        [AddressLevel("Элемент улично-дорожной сети")]
        ElementOfRoadNetwork = 8,
        [AddressLevel("Земельный участок")]
        Land = 9,
        [AddressLevel("Здание (сооружение)")]
        Building = 10,
        [AddressLevel("Помещение")]
        Room = 11,
        [AddressLevel("Помещения в пределах помещения")]
        RoomInRooms = 12,
        [AddressLevel("Уровень автономного округа")]
        AutonomousRegionLevel = 13,
        [AddressLevel("Уровень внутригородской территории")]
        IntracityLevel = 14,
        [AddressLevel("Уровень дополнительных территорий")]
        AdditionalTerritoriesLevel = 15,
        [AddressLevel("Уровень объектов на дополнительных территориях")]
        LevelOfObjectsInAdditionalTerritories = 16,
        [AddressLevel("Машиноместо")]
        CarPlace = 17 

    }

    
}