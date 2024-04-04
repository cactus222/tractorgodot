using System.Collections.Generic;
public class SinglePlayDetails {
    private List<Card> forcedPlays;
    private bool bl;
    private int ptValue;
    private int ptStr;

    public SinglePlayDetails() {
        ptValue = 0;
        ptStr = 0;
        bl = false;
        forcedPlays = new List<Card>();
    }

    public SinglePlayDetails(SinglePlayDetails copy) {
        ptValue = 0;
        bl = false;
        forcedPlays = new List<Card>(copy.forcedPlays);
    }
    public void setCards(List<Card> cards) {
        this.forcedPlays = cards;
        this.ptValue = CardUtils.getTotalPoints(this.forcedPlays);
        //TODO card strengths, ex dont break pairs, 2s crap, ace good.
    }

    public List<Card> getCards() {
        return forcedPlays;
    }

    public int getPointValue() {
        return ptValue;
    }

    public int getCardStrengths() {
        return ptStr;
    }
    public void setBeatsLeader(bool b) {
        this.bl = b;
    }
    public bool beatsLeader() {
        return bl;
    }

}