package com.upokecenter.mail;

    /**
     * Stores an arbitrary string and a "quality value" for that string. For
     * instance, the string can be a language tag, and the "quality value"
     * can be the degree of preference for that language.
     */
    public final class StringAndQuality {
    /**
     * Initializes a new instance of the {@link StringAndQuality} class.
     * @param value A string object.
     * @param quality A 32-bit signed integer.
     */
      public StringAndQuality(String value, int quality) {
        this.propVarvalue = value;
        this.propVarquality = quality;
      }

    /**
     * Gets the arbitrary string stored by this object.
     * @return The arbitrary string stored by this object.
     */
      public final String getValue() { return propVarvalue; }
private final String propVarvalue;

    /**
     * Gets the quality value stored by this object.
     * @return The quality value stored by this object.
     */
      public final int getQuality() { return propVarquality; }
private final int propVarquality;
    }
