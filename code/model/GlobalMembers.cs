using System.Collections.Generic;
using System.Linq;

public static class GlobalMembers {
	public static readonly int NUM_SUITS = 4;

	public static readonly Dictionary<Suit, int> SUIT_MAPPING = new Dictionary<Suit, int>() { { Suit.DIAMONDS, 0 }, { Suit.CLUBS, 1 }, { Suit.HEARTS, 2 }, { Suit.SPADES, 3 } };
	public static readonly Suit[] BASIC_SUITS = { Suit.DIAMONDS, Suit.CLUBS, Suit.HEARTS, Suit.SPADES };
	public static readonly Suit[] BASIC_SUITS_WITH_TRUMP = { Suit.DIAMONDS, Suit.CLUBS, Suit.HEARTS, Suit.SPADES, Suit.TRUMP };
	public static readonly Dictionary<Suit, int> SUIT_MAPPING_WITH_JOKERS = new Dictionary<Suit, int>() { { Suit.DIAMONDS, 0 }, { Suit.CLUBS, 1 }, { Suit.HEARTS, 2 }, { Suit.SPADES, 3 }, { Suit.JOKER, 4 } };

	public static readonly Dictionary<Suit, string> SUIT_STRING_MAPPING = new Dictionary<Suit, string>() { { Suit.DIAMONDS, "DIAMONDS" }, { Suit.CLUBS, "CLUBS" }, { Suit.HEARTS, "HEARTS" }, { Suit.SPADES, "SPADES" }, { Suit.JOKER, "JOKER" } };
	public static readonly Dictionary<Suit, string> TRUMP_SUIT_STRING_MAPPING = new Dictionary<Suit, string>() { { Suit.DIAMONDS, "DIAMONDS" }, { Suit.CLUBS, "CLUBS" }, { Suit.HEARTS, "HEARTS" }, { Suit.SPADES, "SPADES" }, { Suit.NO_TRUMP, "NO TRUMP" } };

	public static readonly Dictionary<Rank, string> RANK_STRING_MAPPING = new Dictionary<Rank, string>(){
		{ Rank.TWO, "2" },
	{ Rank.THREE, "3" },
	{ Rank.FOUR, "4" },
	{ Rank.FIVE, "5" },
	{ Rank.SIX, "6" },
	{ Rank.SEVEN, "7" },
	{ Rank.EIGHT, "8" },
	{ Rank.NINE, "9" },
	{ Rank.TEN, "T" },
	{ Rank.JACK, "J" },
	{ Rank.QUEEN, "Q" },
	{ Rank.KING, "K" },
	{ Rank.ACE, "A" },
	{Rank.JOKER_UNC, "S"}, // Small joker
	{Rank.JOKER_COL, "B"}}; // big joker

	public static readonly Dictionary<string, Rank> STRING_RANK_MAPPING = RANK_STRING_MAPPING.ToDictionary(x => x.Value, x => x.Key);
	public static readonly Dictionary<string, Suit> SIMPLE_SUIT_STRING_SUIT_MAPPING = SUIT_STRING_MAPPING.ToDictionary(x => x.Value.Substring(0, 1), x => x.Key);


	public static readonly Dictionary<Rank, int> RANK_MAPPING = new Dictionary<Rank, int>(){
		{Rank.TWO, 2},
		{Rank.THREE, 3},
		{Rank.FOUR, 4},
		{Rank.FIVE, 5},
		{Rank.SIX, 6},
		{Rank.SEVEN, 7},
		{Rank.EIGHT, 8},
		{Rank.NINE, 9},
		{Rank.TEN, 10},
		{Rank.JACK, 11},
		{Rank.QUEEN, 12},
		{Rank.KING, 13},
		{Rank.ACE, 14}
		// ,
		// {JOKER_UNC, 15},
		// {JOKER_COL, 16}
		
	};


	public static readonly Dictionary<Rank, int> RANK_MAPPING_WITH_JOKERS = new Dictionary<Rank, int>(){
		{ Rank.TWO, 2 },
		{ Rank.THREE, 3 },
		{ Rank.FOUR, 4 },
		{ Rank.FIVE, 5 },
		{ Rank.SIX, 6 },
		{ Rank.SEVEN, 7 },
		{ Rank.EIGHT, 8 },
		{ Rank.NINE, 9 },
		{ Rank.TEN, 10 },
		{ Rank.JACK, 11 },
		{ Rank.QUEEN, 12 },
		{ Rank.KING, 13 },
		{ Rank.ACE, 14 },
		{Rank.JOKER_UNC, 15},
		{Rank.JOKER_COL, 16}
	};

	public static readonly Dictionary<int, Rank> INVERSE_RANK_MAPPING = new Dictionary<int, Rank>(){
		{2, Rank.TWO},
		{3, Rank.THREE},
		{4, Rank.FOUR},
		{5, Rank.FIVE},
		{6, Rank.SIX},
		{7, Rank.SEVEN},
		{8, Rank.EIGHT},
		{9, Rank.NINE},
		{10, Rank.TEN},
		{11, Rank.JACK},
		{12, Rank.QUEEN},
		{13, Rank.KING},
		{14, Rank.ACE},
		{15, Rank.JOKER_UNC},
		{16, Rank.JOKER_COL}

	};

}
