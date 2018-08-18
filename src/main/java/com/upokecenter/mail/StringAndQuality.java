package com.upokecenter.mail;

    /**
     * Not documented yet.
     */
    public final class StringAndQuality {
    /**
     * Initializes a new instance of the StringAndQuality class.
     * @param value A string object.
     * @param quality A 32-bit signed integer.
     */
      public StringAndQuality(String value, int quality) {
        this.propVarvalue = value;
        this.propVarquality = quality;
      }

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
      public final String getValue() { return propVarvalue; }
private final String propVarvalue;

    /**
     * Gets a value not documented yet.
     * @return A value not documented yet.
     */
      public final int getQuality() { return propVarquality; }
private final int propVarquality;
    }
