using System.Collections;

public class PlayUnitComparator : IComparer {

    public int Compare(object a, object b) {
        PlayUnit p1 = (PlayUnit)a;
        PlayUnit p2 = (PlayUnit)b;
        if (p1.getMode() > p2.getMode()) {
            return 1;
        }
        if (p2.getMode() > p1.getMode()) {
            return -1;
        }
        //Modes same
        if (p1.getMode() == PlayUnit.SINGLE_MODE) {
            return 0;
        } else if (p1.getMode() == PlayUnit.PAIR_MODE) {
            return 0;
        } else if (p1.getMode() == PlayUnit.TRACTOR_MODE) {
            if (p1.getTractorSize() > p2.getTractorSize()) {
                return 1;
            } else if (p1.getTractorSize() == p2.getTractorSize()) {
                return 0;
            }
            return -1;
        }
        return 0;
    }
}
