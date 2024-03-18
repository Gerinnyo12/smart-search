# Keresés típusok

_Az alábbi típusok közül mindegyik case insensitive_ <br>
_"kifejezés" = 1 vagy több szó_

## BoolPrefix:
- Gyors
- Nem kis és nagybetű érzékeny
- Feldarabolja a keresendő kifejezést szöközök mentén egy N elemű szó listára
- A találatok azokkal az kifejezésekkel fognak egyezni, ahol:
	- Szerepel a találatban olyan szó, ami egy az egyben ugyan az, mint a feldarabolt szó lista első N-1 eleméből bármelyik
	- **vagy** 
	- Szerepel a találatban olyan szó, ami ugyan úgy kezdődik, mint a feldarabolt szó lista N. eleme
- Példa: 
	- Keresendő kifejezés: `sajtos pi`
	- Találatok: 
		- Sajtos pizza
		- A kenyér sajtos.
		- pisztácia

## PhrasePrefix:
- Gyors
- Nem kis és nagybetű érzékeny
- Feldarabolja a keresendő kifejezést szöközök mentén egy N elemű szó listára
- A találatok azokkal a kifejezésekkel fog egyezni, ahol:
	- Szerepel az első N-1 szó a találatban, a megadott sorrendben
	- **és**
	- Szerepel a találatban olyan szó, ami ugyan úgy kezdődik, mint a feldarabolt szó lista N. eleme
- Példa: 
	- Keresendő kifejezés: `sajtos pi`
	- Találatok: 
		- Sajtos pizza
		- Kicsi sajtos pisztáciák.

## Wildcard:
- Lassú
- Nem kis és nagybetű érzékeny
- A találatok azokkal a kifejezésekkel fog egyezni, ahol:
	- Szerepel a találatban a keresendő kifejezés úgy, hogy előtte és utána is bármelyik karakterből bármennyi szerepelhet
- Példa:
	- Keresendő kifejezés: `sajtos pi`
	- Találatok:
		- Sajtos pizza
		- A közepes méretű sajtos pizza.
		- nagy sajtos pizza és kis sonkás szendvics
- Megjegyzés:
	- Ez a keresés típus felel meg SQL-ben a `LIKE '%sajtos pi%'` szintaxisnak

## Prefix:
- Gyors
- Nem kis és nagybetű érzékeny
- A találatok azokkal a kifejezésekkel fog egyezni, ahol:
	- A találat úgy kezdődik, mint a keresendő kifejezés
- Példa:
	- Keresendő kifejezés: `sajtos pi`
	- Találatok:
		- Sajtos pizza
		- sajtos pisztáciás szendvics
- Megjegyzés:
	- Ez a keresés típus felel meg SQL-ben a `LIKE 'sajtos pi%'` szintaxisnak

## Term:

- Gyors
- Nem kis és nagybetű érzékeny
- A találatok azokkal a kifejezésekkel fog egyezni, ahol:
	- A találat pontosan ugyan az, mint a keresendő kifejezés
- Példa:
	- Keresendő kifejezés: `sajtos pi`
	- Találatok:
		- sajtos pi
- Megjegyzés:
	- Ez a keresés típus felel meg SQL-ben az `= 'sajtos pi'` szintaxisnak
