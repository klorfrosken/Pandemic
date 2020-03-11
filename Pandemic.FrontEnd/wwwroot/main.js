const mapObject = document.getElementById("map-object");
let map = null;
console.log(mapObject);
function reportMapSize() {
    console.log(map);
    if (map != null) {
        console.log(map.getBoundingClientRect());
    }
}

mapObject.addEventListener("load", function () {
    const mapDocument = mapObject.contentDocument;
    console.log(mapDocument);
    map = mapDocument.getElementById("background-map");
});
window.onresize = reportMapSize;


// <div id="map">
//    <object type="image/svg+xml" data="/css/icons/BackgroundMap.svg" id="map-object" class="map-svg">
//    </object>
//    @foreach (City city in currentState.Cities.Values)
//            {
//        <span class="city-span" id="@setCityID(city)">
//            <span class="city-icon-and-disease">
//                <img class="city-image" src="/css/icons/Gamma AlfaDisease.png" alt=@city.Name srcset="/css/icons/Gamma AlfaDisease.svg" />
//                        <span class="city-disease-cubes">{@city.DiseaseCubes[city.Color]}</span>
//            </span>

//            <span class="city-name">@city.Name</span>
//        </span>
//    }
//</div> *@
//            @* <div id="what-to-do-next-box">
//    Waiting for you to: <br />
//    ...text...<br />
//    (Remaining Actions)
//            </div>

//    <div id="infection-discard-div">
//        <img class="cards" id="infection-discard-img" src="/css/icons/No InfectionCard.png" alt="Infection deck discard pile" srcset="/css/icons/No InfectionCard.svg" />
//    </div>