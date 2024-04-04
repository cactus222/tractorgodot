using Godot;
using System;
using System.Collections.Generic;

public class ImageLibrary {

	//static readonly string CARD_BASE_LOC = "res://res/playing-cards-assets/svg-cards/";
	static readonly string CARD_BASE_LOC = "res://res/pngcards/";
	//static readonly string FILE_TYPE = ".svg";
	static readonly string FILE_TYPE = ".png";
	static Dictionary <Card, Texture2D> cardLibrary = null;
	
	public static Texture2D GetCardImage(Card c) {
		if (cardLibrary == null) {
			getCardLibraryDict();
		}
		return cardLibrary[c];
	}

	private static void getCardLibraryDict()
	{
		cardLibrary = new Dictionary<Card, Texture2D>();
		foreach(KeyValuePair<Rank, string> rankKVP in RANK_TO_CARD_NAME_MAPPING) {
			foreach(KeyValuePair<Suit, string> suitKVP in GlobalMembers.SUIT_STRING_MAPPING) {
				Rank rank = rankKVP.Key;
				Suit suit = suitKVP.Key;
				if (suit == Suit.JOKER || rank == Rank.JOKER_UNC || rank == Rank.JOKER_COL) {
					continue;
				}
				Card card = new Card(rank, suit);
				string cardString = rankKVP.Value + "_of_" + suitKVP.Value.ToLower();
				Texture2D spr = (Texture2D)GD.Load(CARD_BASE_LOC + cardString + FILE_TYPE);
				if (spr != null) {
					cardLibrary[card] = spr; 
				}
			}
		}

		string jokerstring = "black_joker";
		Texture2D spriteJoker = (Texture2D)GD.Load(CARD_BASE_LOC + jokerstring + FILE_TYPE);
		cardLibrary[CardUtils.generateJoker(Rank.JOKER_UNC)] = spriteJoker; 

		jokerstring = "red_joker";
		spriteJoker = (Texture2D)GD.Load(CARD_BASE_LOC + jokerstring + FILE_TYPE);
		cardLibrary[CardUtils.generateJoker(Rank.JOKER_COL)] = spriteJoker; 
	}
	private static readonly Dictionary<Rank, string> RANK_TO_CARD_NAME_MAPPING = new Dictionary<Rank, string>(){
	{ Rank.TWO, "2" },
	{ Rank.THREE, "3" },
	{ Rank.FOUR, "4" },
	{ Rank.FIVE, "5" },
	{ Rank.SIX, "6" },
	{ Rank.SEVEN, "7" },
	{ Rank.EIGHT, "8" },
	{ Rank.NINE, "9" },
	{ Rank.TEN, "10" },
	{ Rank.JACK, "jack" },
	{ Rank.QUEEN, "queen" },
	{ Rank.KING, "king" },
	{ Rank.ACE, "ace" }};
}

