using System.Collections.Generic;
//TODO WDF IS THIS

public class PlayUnit {
    private List<Card> cards = new List<Card>();
    private int mode = MODE_ERROR;


    public override string ToString() {
        string msg = "";
        switch(mode) {
            case TRACTOR_MODE:
                msg += " tractor ";
                break;
            case PAIR_MODE:
                msg += " pair ";
                break;
            case SINGLE_MODE:
                msg += " single ";
                break;
            default:
                msg += "wdf?";
                break;
        }

        msg += $" {CardUtils.getCardListString(cards)} ";

        return msg;
    }
    public int getTractorSize() {
        if (isTractor()) {
            return cards.Count / 2;
        }
        return 0;
    }
    public bool isTractor() {
        return mode == TRACTOR_MODE;
    }
    public int getPlayUnitType() {
        return mode;
    }
    public PlayUnit(List<Card> cs, int t) {
        init(cs, t);
    }
    public PlayUnit(Card cs, int t) {
        List<Card> css = new List<Card>();
        css.Add(cs);
        init(css, t);
    }
    public void init(List<Card> cs, int t) {
        foreach (Card c in cs) {
            if (c == null) {
                throw new System.Exception("WDF EXCEPTION");
            }
        }
        if (cs == null || cs.Count == 0) {
            throw new System.Exception("WDF EMPTY PLAYUNIT?");
        }
        mode = t;
        cards = new List<Card>(cs);
        if (mode == TRACTOR_MODE && cards.Count < 4) {
            throw new System.Exception("WHAT THE, tractor less than 4 ${CardUtils.getCardListString(cards)}");
        }
    }
    public Card getHighestCard() {
        return cards[cards.Count - 1];
    }
    public bool isAllOfSameSuit() {
        Suit s = CardUtils.getSuitFactorTrump(cards[0]);
        for (int i = 1; i < cards.Count; i++) {
            if (CardUtils.getSuitFactorTrump(cards[i]) != s) {
                return false;
            }
        }
        return true;
    }
    public int getLength() {
        return cards.Count;
    }
    public int getMode() {
        return mode;
    }
    public const int SINGLE_MODE = 3;
    public const int PAIR_MODE = 1;
    public const int TRACTOR_MODE = 2;
    public const int MODE_ERROR = 0;



    //TRACTOR
    //TRACTOR LENGTH
    //PAIR
    //# PAIR
    //SINGLE
}