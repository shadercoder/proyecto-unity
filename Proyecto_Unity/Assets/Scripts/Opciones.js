#pragma strict

private var musicaOn : boolean = true;	//Está la música activada?
private var musicaVol : float = 0.5;	//A que volumen?
private var sfxOn : boolean = true;		//Estan los efectos de sonido activados?
private var sfxVol : float = 0.5; 		//A que volumen?

//Getters y setters para las variables
function setMusicaOn(sel : boolean) {
	musicaOn = sel;
}

function setMusicaVol(sel : float) {
	musicaVol = Mathf.Clamp01(sel);
}

function setSfxOn(sel : boolean) {
	sfxOn = sel;
}

function setSfxVol(sel : float) {
	sfxVol = Mathf.Clamp01(sel);
}

function getMusicaOn() {
	return musicaOn;
}

function getMusicaVol() {
	return musicaVol;
}

function getSfxOn() {
	return sfxOn;
}

function getSfxVol() {
	return sfxVol;
}
