<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:output method="xml" indent="yes" encoding="utf-8"/>
	<xsl:strip-space elements="*"/>

	<xsl:key name="employee-key" match="item" use="concat(@name, '|', @surname)"/>

	<xsl:template match="/">
		<Employees>
			<xsl:apply-templates
                select="Pay//item[generate-id() = generate-id(key('employee-key', concat(@name, '|', @surname))[1])]">
				<xsl:sort select="@surname"/>
				<xsl:sort select="@name"/>
			</xsl:apply-templates>
		</Employees>
	</xsl:template>

	<xsl:template match="item">
		<Employee name="{@name}" surname="{@surname}">
			<xsl:for-each select="key('employee-key', concat(@name, '|', @surname))">
				<salary amount="{@amount}" mount="{@mount}"/>
			</xsl:for-each>
		</Employee>
	</xsl:template>

</xsl:stylesheet>