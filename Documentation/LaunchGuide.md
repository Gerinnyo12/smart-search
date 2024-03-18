# Indítási útmutató

## Appsettings.json konfigurálása:
- `ElasticsearchClientOptions` szakasz
    - Az Elasticsearch szerver eléréséhez szükséges adatok beállítása
    - `Url`: Az Elasticsearch szerver elérését biztosító API url-je
    - `Fingerprint`: Az Elasticsearch szerver publikus SHA256 kulcsa hexadecimális formában
    - `Username`: Az Elasticsearch szerver authentikációjához szükséges felhasználó név
    - `Password`: Az Elasticsearch szerver authentikációjához szükséges jelszó
    - `IndexOptions` szakasz:
        - Az index konfigurálásához szükséges adatok beállítása
        - `IndexName`: A használni kívánt index neve
        - `NumberOfShards`: Az index adatának elosztása ennyi darab shard-ba
            - Ajánlott érték: 3
        - `NumberOfReplicas`: Az index adatáról készült biztonsági másolat elosztása ennyi darab shard-ba
            - Erősen ajánlott érték: 0
- `ElasticsearchData` szakasz:
    - Az Elasticsearch által használt adatok eléréseinek beállítása
    - `BatchSize`: Egy "csomagban" mozgó rekordok száma szinkronizációkor
    - `ElasticTargets`: 
        - Tömb
        - Egy elemének tartalma:
            - `ConnectionString`: Az adatbázis eléréséhez szükséges kapcsolódási string
            - `Server`: Az adatbázis szerver neve
            - `Database`: Az adatbázis szerveren található adatbázis neve
            - `Tables`:
                - Tömb
                - Egy elemének tartalma:
                    - `Table`: Az adatbázis táblájának neve
                    - `Type`: 
                        - A tábla típusa
                        - A lehetséges értékei a `TableType` enum értékei
                    - `Keys`: 
                        - Tömb
                        - A tábla kulcsainak nevei
                    - `Columns`: 
                        - Tömb
                        - A tábla oszlopainak nevei
    - Példa:
        ```
        "ElasticsearchData": {
            "BatchSize": 5000,
            "ElasticTargets": [
                {
                    "ConnectionString": "Server=ExampleServer; Database=ExampleDatabase; ExampleAuthentication; TrustServerCertificate=True; MultipleActiveResultSets=True;",
                    "Server": "ExampleServer",
                    "Database": "ExampleDatabase",
                    "Tables": [
                        {
                            "Table": "ExampleTableName",
                            "Type": "First",
                            "Keys": [ "FirstKey", "SecondKey" ],
                            "Columns": [ "FirstTextColumn", "SecondTextColumn" ]
                        }, 
                        ...
                    ]
                }, 
                ...
            ]
        }
        ```

- `CronSchedulerOptions`:
    - A szinkronizációhoz szükséges adatok beállítása
    - `Cron`: 
        - A szinkronizáció futtatásának időpontjai
        - 5 elemű, másodperc rész nélküli cron kifejezés


## Fejlesztői beállítások:
- A GriffSoft.SmartSearch/GriffSoft.SmartSearch.Frontend útvonalon található `Program.cs` file-ban szereplő `ISynchronizerService` beállítása egy megfelelő implementációs típusra

## Indítás
A fent leírt konfigurációk elvégzése után a `dotnet run --project ./GriffSoft.SmartSearch/GriffSoft.SmartSearch.Frontend` parancs kiadásával elindul egy Blazor felhasználói felület, ami a _TODO_ url-en keresztül érhető el