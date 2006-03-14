﻿<?xml version="1.0" encoding="utf-8"?>
<!--
	Neumont Object Role Modeling Architect for Visual Studio

	Copyright © Neumont University. All rights reserved.

	The use and distribution terms for this software are covered by the
	Common Public License 1.0 (http://opensource.org/licenses/cpl) which
	can be found in the file CPL.txt at the root of this distribution.
	By using this software in any fashion, you are agreeing to be bound by
	the terms of this license.

	You must not remove this notice, or any other, from this software.
-->
<xsl:stylesheet version="1.0"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
	xmlns:exsl="http://exslt.org/common"
	xmlns:oil="http://schemas.orm.net/OIAL"
	xmlns:odt="http://schemas.orm.net/ORMDataTypes"
	xmlns:xsOut="OutputSchema"
	xmlns:xs="http://www.w3.org/2001/XMLSchema" 
	extension-element-prefixes="exsl"
	exclude-result-prefixes="oil odt">
	<xsl:namespace-alias stylesheet-prefix="xsOut" result-prefix="xs"/>

	<xsl:output method="xml" encoding="utf-8" media-type="text/xml" indent="yes"/>
	<xsl:strip-space elements="*"/>

	<xsl:template name="AddNamespacePrefix">
		<xsl:param name="Prefix"/>
		<xsl:param name="Namespace"/>
		<xsl:variable name="DummyFragment">
			<xsl:choose>
				<xsl:when test="string-length($Prefix)">
					<xsl:element name="{$Prefix}:PickAName" namespace="{$Namespace}"/>
				</xsl:when>
				<xsl:otherwise>
					<xsl:element name="PickAName" namespace="{$Namespace}"/>
				</xsl:otherwise>
			</xsl:choose>
		</xsl:variable>
		<xsl:copy-of select="exsl:node-set($DummyFragment)/child::*/namespace::*[local-name()!='xml']"/>
	</xsl:template>
	<xsl:template match="oil:model">
		<xsl:comment>THIS SCHEMA WAS GENERATED BY THE ORM2 TOOL</xsl:comment>
		<xsOut:schema>
			<xsl:call-template name="AddNamespacePrefix">
				<xsl:with-param name="Prefix" select="'oxs'"/>
				<xsl:with-param name="Namespace" select="concat('http://schemas.neumont.edu/ORM/CodeGeneratedSchema/',@name)"/>
			</xsl:call-template>
			<xsl:call-template name="AddNamespacePrefix">
				<xsl:with-param name="Prefix" select="''"/>
				<xsl:with-param name="Namespace" select="concat('http://schemas.neumont.edu/ORM/CodeGeneratedSchema/',@name)"/>
			</xsl:call-template>
			<xsl:attribute name="id">
				<xsl:value-of select="@name"/>
			</xsl:attribute>
			<xsl:attribute name="elementFormDefault">qualified</xsl:attribute>
			<xsl:attribute name="targetNamespace">
				<xsl:text>http://schemas.neumont.edu/ORM/CodeGeneratedSchema/</xsl:text>
				<xsl:value-of select="@name"/>
			</xsl:attribute>
			<xsl:attribute name="version">1.0</xsl:attribute>
			<xsl:comment>Each ORM ValueType with a value/range/length constraint</xsl:comment>
			<xsl:variable name="InformationTypeFormatMappingsFragment">
				<xsl:apply-templates select="oil:informationTypeFormats/child::*" mode="GenerateMapping"/>
			</xsl:variable>
			<xsl:variable name="InformationTypeFormatMappings" select="msxsl:node-set($InformationTypeFormatMappingsFragment)/child::*"/>
			<xsl:variable name="ConceptTypes" select="//oil:conceptType"/>
			<xsl:apply-templates select="oil:informationTypeFormats/child::*[@name=$InformationTypeFormatMappings[starts-with(@target,'oxs')]/@name]" mode="GenerateSimpleType"/>
			<xsl:comment>Each ORM EntityType and it's Preferred Id or top level ValueType</xsl:comment>
			<xsl:call-template name="GenerateEntityIdentifierComplexTypes">
				<xsl:with-param name="ConceptTypes" select="$ConceptTypes"/>
				<xsl:with-param name="InformationTypeFormatMappings" select="$InformationTypeFormatMappings"/>
			</xsl:call-template>
			<xsl:comment>Each ORM Major Object Type grouping</xsl:comment>
			<xsl:apply-templates select="oil:conceptType" mode="GenerateMajorEntityElements">
				<xsl:with-param name="ConceptTypes" select="$ConceptTypes"/>
				<xsl:with-param name="InformationTypeFormatMappings" select="$InformationTypeFormatMappings"/>
			</xsl:apply-templates>
			<xsl:comment>Each ORM Major Object Type in the '<xsl:value-of select="@name"/>' schema</xsl:comment>
			<xsOut:element name="{@name}">
				<xsOut:complexType>
					<xsOut:all>
						<xsl:for-each select="oil:conceptType">
							<xsOut:element name="{@name}Elements">
								<xsOut:complexType>
									<xsOut:choice minOccurs="0" maxOccurs="unbounded">
										<xsOut:element name="{@name}" type="oxs:{@name}_FACTS" />
									</xsOut:choice>
								</xsOut:complexType>
							</xsOut:element>
						</xsl:for-each>
					</xsOut:all>
				</xsOut:complexType>
				<xsl:comment>Key Constraints</xsl:comment>
				<xsl:call-template name="GenerateKeyConstraints"/>
				<xsl:comment>Uniqueness Constraints</xsl:comment>
				<xsl:call-template name="GenerateUniquenessConstraints"/>
				<xsl:comment>KeyRefs between Major Object Type groupings</xsl:comment>
				<xsl:call-template name="GenerateKeyRefs"/>
			</xsOut:element>
		</xsOut:schema>
	</xsl:template>

	<xsl:template match="odt:identity" mode="GenerateMapping">
		<FormatMapping name="{@name}" target="xs:integer"/>
	</xsl:template>
	<xsl:template match="odt:boolean" mode="GenerateMapping">
		<FormatMapping name="{@name}">
			<xsl:attribute name="target">
				<xsl:choose>
					<xsl:when test="@fixed='true'">
						<xsl:value-of select="'oxs:true'"/>
					</xsl:when>
					<xsl:when test="@fixed='false'">
						<xsl:value-of select="'oxs:false'"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="'xs:boolean'"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
		</FormatMapping>
	</xsl:template>
	<xsl:template match="odt:decimalNumber" mode="GenerateMapping">
		<FormatMapping name="{@name}">
			<xsl:attribute name="target">
				<xsl:choose>
					<!-- TODO: Optimize this so that we map to smaller integer types when possible. -->
					<xsl:when test="odt:enumeration or odt:range or @totalDigits or not(@fractionDigits=0)">
						<xsl:value-of select="concat('oxs:', @name)"/>
					</xsl:when>
					<xsl:when test="@fractionDigits = 0">
						<xsl:value-of select="'xs:integer'"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="'xs:decimal'"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
		</FormatMapping>
	</xsl:template>
	<xsl:template match="odt:floatingPointNumber" mode="GenerateMapping">
		<FormatMapping name="{@name}">
			<xsl:attribute name="target">
				<xsl:choose>
					<xsl:when test="child::*">
						<xsl:value-of select="concat('oxs:', @name)"/>
					</xsl:when>
					<xsl:when test="@precision &lt; 25 or @precision='single'">
						<xsl:value-of select="'xs:float'"/>
					</xsl:when>
					<xsl:when test="@precision &lt; 54 or @precision='double'">
						<xsl:value-of select="'xs:double'"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:message terminate="yes">
							<xsl:text>Sorry, XML Schema doesn't support floating point data types above double-precision.</xsl:text>
						</xsl:message>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
		</FormatMapping>
	</xsl:template>
	<xsl:template match="odt:string" mode="GenerateMapping">
		<FormatMapping name="{@name}">
			<xsl:attribute name="target">
				<xsl:choose>
					<xsl:when test="@minLength or @maxLength or child::*">
						<xsl:value-of select="concat('oxs:', @name)"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="'xs:string'"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
		</FormatMapping>
	</xsl:template>
	<xsl:template match="odt:binary" mode="GenerateMapping">
		<FormatMapping name="{@name}">
			<xsl:attribute name="target">
				<xsl:choose>
					<xsl:when test="@minLength or @maxLength">
						<xsl:value-of select="concat('oxs:', @name)"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:value-of select="'xs:hexBinary'"/>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:attribute>
		</FormatMapping>
	</xsl:template>

	<xsl:template match="oil:informationTypeFormats/child::*" mode="GenerateSimpleType">
		<!-- informationTypeFormats that don't have a one-to-one mapping to predefined data types are made into xs:simpleTypes -->
		<xsOut:simpleType name="{@name}">
			<xsl:apply-templates select="." mode="GenerateSimpleTypeRestriction"/>
		</xsOut:simpleType>
	</xsl:template>

	<xsl:template match="odt:boolean" mode="GenerateSimpleTypeRestriction">
		<xsOut:restriction base="xs:boolean">
			<xsOut:enumeration value="{@fixed}"/>
		</xsOut:restriction>
	</xsl:template>
	<xsl:template match="odt:decimalNumber" mode="GenerateSimpleTypeRestriction">
		<!-- TODO: Need to finish this. -->
		<xsOut:restriction base="xs:decimal">
			<xsOut:fractionDigits value="{@fractionDigits}"/>
			<xsl:if test="@totalDigits">
				<xsOut:totalDigits value="{@totalDigits}"/>
			</xsl:if>
			<xsl:if test="odt:range">
				<xsl:apply-templates select="odt:range"/>
			</xsl:if>

		</xsOut:restriction>
		<!-- Stuff relating to having multiple Ranges or enumerations-->
	</xsl:template>
	<xsl:template match="odt:floatingPointNumber" mode="GenerateSimpleTypeRestriction">
		<!-- TODO: Need to finish this. -->
		<!-- Stuff relating to having multiple Ranges or enumerations-->
		<xsOut:restriction base="xs:float">
			<xsl:if test="@precision">

			</xsl:if>
		</xsOut:restriction>
	</xsl:template>
	<xsl:template match="odt:string" mode="GenerateSimpleTypeRestriction">
		<xsOut:restriction base="xs:string">
			<xsl:if test="@minLength">
				<xsOut:minLength value="{@minLength}"/>
			</xsl:if>
			<xsl:if test="@maxLength">
				<xsOut:maxLength value="{@maxLength}"/>
			</xsl:if>
			<xsl:choose>
				<xsl:when test="odt:enumeration and not(odt:pattern)">
					<xsl:for-each select="odt:enumeration">
						<xsOut:enumeration value="{@value}"/>
					</xsl:for-each>
				</xsl:when>
				<xsl:when test="odt:pattern">
					<xsOut:pattern>
						<xsl:attribute name="value">
							<xsl:text>(</xsl:text>
							<xsl:for-each select="child::*">
								<xsl:text>(</xsl:text>
								<xsl:value-of select="@value"/>
								<xsl:text>)</xsl:text>
								<xsl:if test="not(position()=last())">
									<xsl:text>|</xsl:text>
								</xsl:if>
							</xsl:for-each>
							<xsl:text>)</xsl:text>
						</xsl:attribute>
					</xsOut:pattern>
				</xsl:when>
			</xsl:choose>
		</xsOut:restriction>
	</xsl:template>
	<xsl:template match="odt:binary" mode="GenerateSimpleTypeRestriction">
		<xsOut:restriction base="xs:hexBinary">
			<xsl:if test="@minLength">
				<xsOut:minLength value="{@minLength}"/>
			</xsl:if>
			<xsl:if test="@maxLength">
				<xsOut:minLength value="{@maxLength}"/>
			</xsl:if>
		</xsOut:restriction>
	</xsl:template>
	<xsl:template match="*" mode="GenerateSimpleTypeRestriction">
		<xsl:text>WARNING: Unrecognized information type...</xsl:text>
	</xsl:template>

	<xsl:template name="GenerateEntityIdentifierComplexTypes">
		<xsl:param name="ConceptTypes"/>
		<xsl:param name="InformationTypeFormatMappings"/>
		<xsl:for-each select="$ConceptTypes">
			<xsl:call-template name="GetIdentifierAttributesForConceptType">
				<xsl:with-param name="ConceptTypes" select="$ConceptTypes"/>
				<xsl:with-param name="InformationTypeFormatMappings" select="$InformationTypeFormatMappings"/>
				<xsl:with-param name="ConceptTypeTarget" select="."/>
			</xsl:call-template>
		</xsl:for-each>
	</xsl:template>
	
	<xsl:template name="GetIdentifierAttributesForConceptType">
		<xsl:param name="ConceptTypes"/>
		<xsl:param name="InformationTypeFormatMappings"/>
		<xsl:param name="ConceptTypeTarget"/>
		<xsl:param name="IsForReference" select="false()"/>
		<xsl:variable name="nestedIdentifiersFragment">
			<xsl:for-each select="$ConceptTypeTarget/oil:roleSequenceUniquenessConstraint[@isPreferred='true']/oil:roleSequence/oil:typeRef">
				<xsl:variable name="target" select="../../../child::*[@name=current()/@targetChild]"/>
				<xsl:choose>
					<xsl:when test="$target/self::oil:conceptTypeRef">
						<xsl:call-template name="GetIdentifierAttributesForConceptType">
							<xsl:with-param name="ConceptTypes" select="$ConceptTypes"/>
							<xsl:with-param name="InformationTypeFormatMappings" select="$InformationTypeFormatMappings"/>
							<xsl:with-param name="ConceptTypeTarget" select="$ConceptTypes[@name=$target/@target]"/>
							<xsl:with-param name="IsForReference" select="true()"/>
						</xsl:call-template>
					</xsl:when>
					<xsl:when test="$target/self::oil:informationType">
						<xsOut:attribute name="{$target/@name}" use="required" type="{$InformationTypeFormatMappings[@name=$target/@formatRef]/@target}"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:message terminate="yes">
							<xsl:text>SANITY CHECK: An oil:conceptType should never be compositely identified by another oil:conceptType nested within it.</xsl:text>
						</xsl:message>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:for-each>
			<!-- Is that supposed to be oil:informationType on the next line? Can an oil:singleRoleUniquenessConstraint occur inside an oil:conceptTypeRef? -->
			<xsl:for-each select="$ConceptTypeTarget/oil:informationType[oil:singleRoleUniquenessConstraint/@isPreferred='true']">
				<xsOut:attribute name="{@name}" use="required" type="{$InformationTypeFormatMappings[@name=current()/@formatRef]/@target}"/>
			</xsl:for-each>
		</xsl:variable>
		<xsl:variable name="nestedIdentifiers" select="msxsl:node-set($nestedIdentifiersFragment)"/>
		<xsl:choose>
			<xsl:when test="not($IsForReference) and $nestedIdentifiers">
				<xsOut:complexType name="{$ConceptTypeTarget/@name}Identifier">
					<xsl:copy-of select="$nestedIdentifiers"/>
				</xsOut:complexType>
			</xsl:when>
			<xsl:when test="$IsForReference">
				<xsl:choose>
					<xsl:when test="$nestedIdentifiers">
						<xsl:copy-of select="$nestedIdentifiers"/>
					</xsl:when>
					<xsl:otherwise>
						<xsl:variable name="parentConceptType" select="$ConceptTypeTarget/parent::oil:conceptType"/>
						<xsl:if test="not($parentConceptType)">
							<xsl:message terminate="yes">
								<xsl:text>SANITY CHECK: oil:conceptTypes must be identified by a preferred uniqueness constraint or by being nested within another oil:ConceptType.</xsl:text>
							</xsl:message>
						</xsl:if>
						<xsl:call-template name="GetIdentifierAttributesForConceptType">
							<xsl:with-param name="ConceptTypes" select="$ConceptTypes"/>
							<xsl:with-param name="InformationTypeFormatMappings" select="$InformationTypeFormatMappings"/>
							<xsl:with-param name="ConceptTypeTarget" select="$parentConceptType"/>
						</xsl:call-template>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:when>
		</xsl:choose>
	</xsl:template>

	<xsl:template match="oil:conceptType" mode="GenerateMajorEntityElements">
		<xsl:param name="ConceptTypes"/>
		<xsl:param name="InformationTypeFormatMappings"/>
		<!--<xsl:variable name="preferredIdentifier" select="oil:informationType[oil:singleRoleUniquenessConstraint/@isPreferred='true']/@name"/>-->
		<!--Complex type definitions for each major Object Type-->
		<xsOut:complexType name="{@name}Facts">
			<xsOut:complexContent>
				<xsOut:extension base="oxs:{@name}Identifier">
					<xsOut:sequence>
						<xsl:for-each select="oil:conceptType">
							<!--  -->
							<xs:element name="{@name}" type="oxs:{@name}Identifier">
							</xs:element>
						</xsl:for-each>
						<xsl:for-each select="oil:conceptTypeRef">
							<xsOut:element name="{@name}" type="oxs:{@target}Identifier"/>
						</xsl:for-each>
						<!--<xsl:for-each select="oil:informationType">
							<xsl:if test="@mandatory !='alethic'">
								<xs:element name="{@name}" type="{$InformationTypeFormatMappings[@name=current()/@formatRef]/@target}"/>
							</xsl:if>
						</xsl:for-each>-->
						<xsl:apply-templates select="oil:equalityConstraint"/>
						<xsl:apply-templates select="oil:disjunctiveMandatoryConstraint"/>
						<xsl:apply-templates select="oil:exclusionConstraint"/>
						<!--<xsl:apply-templates select="oil:roleSequenceFrequencyConstraint"/>
						<xsl:apply-templates select="oil:ringConstraint"/>
						<xsl:apply-templates select="oil:subsetConstraint"/>-->
					</xsOut:sequence>

					<!--<xsOut:attribute name="{$preferredIdentifier}" type="{concat('oxs:',$preferredIdentifier)}" use="required"/>-->
					<xsl:for-each select="oil:informationType">
						<xsl:choose>
							<xsl:when test="oil:singleRoleUniquenessConstraint/@isPreferred='true'">
							</xsl:when>
							<xsl:otherwise>
								
								<xsOut:attribute name="{@formatRef}" type="{$InformationTypeFormatMappings[@name=current()/@formatRef]/@target}">
									<xsl:attribute name="use">
										<xsl:choose>
											<xsl:when test="@mandatory = 'alethic'">
												<xsl:value-of select="'required'"/>
											</xsl:when>
											<xsl:otherwise>
												<xsl:value-of select="'optional'"/>
											</xsl:otherwise>
										</xsl:choose>
									</xsl:attribute>
								</xsOut:attribute>
							</xsl:otherwise>
						</xsl:choose>
					</xsl:for-each>
				</xsOut:extension>
			</xsOut:complexContent>
		</xsOut:complexType>
	</xsl:template>

	<xsl:template match="oil:disjunctiveMandatoryConstraint">
		<xsOut:choice minOccurs="1">
			<xsOut:annotation>
				<xsOut:documentation>Disjunctive Mandatory Constraint(Inclusive OR)</xsOut:documentation>
			</xsOut:annotation>
			<xsl:for-each select="oil:roleSequence/oil:typeRef">
				<xsl:variable name="elementName" select="@informationTypeTarget"/>
				<xsl:variable name="elementType" select="concat('oxs:',$elementName)"/>
				<xsOut:element name="{$elementName}" type="{$elementType}">
				</xsOut:element>
			</xsl:for-each>
		</xsOut:choice>
	</xsl:template>
	<xsl:template match="oil:equalityConstraint">
		<xsOut:sequence minOccurs="0">
			<xsOut:annotation>
				<xsOut:documentation>Equality Constraint(All must exist if one exists)</xsOut:documentation>
			</xsOut:annotation>
			<xsl:for-each select="oil:roleSequence">
				<xsl:variable name="elementName" select="oil:typeRef/@informationTypeTarget"/>
				<xsl:variable name="elementType" select="concat('oxs:',$elementName)"/>
				<xsOut:element name="{$elementName}" type="{$elementType}">
				</xsOut:element>
			</xsl:for-each>
		</xsOut:sequence>
	</xsl:template>
	<xsl:template match="oil:roleSequenceFrequencyConstraint">
	</xsl:template>
	<xsl:template match="oil:exclusionConstraint">
		<xsOut:choice minOccurs="0" maxOccurs="1">
			<xsOut:annotation>
				<xsOut:documentation>Exclusion Constraint(XOR)</xsOut:documentation>
			</xsOut:annotation>
			<xsl:for-each select="oil:roleSequence">
				<xsl:variable name="elementName" select="oil:typeRef/@informationTypeTarget"/>
				<xsl:variable name="elementType" select="concat('oxs:',$elementName)"/>
				<xsOut:element name="{$elementName}" type="{$elementType}">
				</xsOut:element>
			</xsl:for-each>
		</xsOut:choice>
	</xsl:template>
	<xsl:template match="oil:valueConstraint">
		<xsl:variable name="rangeCount" select="count(oil:range)"/>
		<xsl:variable name="valueCount" select="count(oil:value)"/>
		<xsl:variable name="value" select="oil:value/@value"/>
		<!-- Check all value constraints for more multiple ranges or values-->
		<xsl:if test="$rangeCount = 0 and $valueCount > 0">
			<xsOut:restriction>
				<xsl:attribute name="base">
					<xsl:call-template name="GenerateXsdDataType"/>
				</xsl:attribute>
				<!-- check for length -->
				<xsOut:enumeration value="{$value}"/>
			</xsOut:restriction>
		</xsl:if>
		<xsl:if test="$rangeCount > 1 and $valueCount = 0">
			<xsOut:union>
				<xsl:for-each select="oil:range">
					<xsOut:simpleType>
						<xsOut:restriction>
							<xsl:attribute name="base">
								<xsl:call-template name="GenerateXsdDataType"/>
							</xsl:attribute>
							<!-- check for length -->
							<xsl:apply-templates select="."/>
						</xsOut:restriction>
					</xsOut:simpleType>
				</xsl:for-each>
			</xsOut:union>
		</xsl:if>
		<xsl:if test="$rangeCount != 0 and $valueCount != 0">
			<xsOut:union>
				<xsl:for-each select="oil:range">
					<xsOut:simpleType>
						<xsOut:restriction>
							<xsl:attribute name="base">
								<xsl:call-template name="GenerateXsdDataType"/>
							</xsl:attribute>
							<!-- check for length -->
							<xsl:apply-templates select="oil:range"/>
						</xsOut:restriction>
					</xsOut:simpleType>
				</xsl:for-each>
				<xsl:for-each select="oil:value">
					<xsOut:simpleType>
						<xsOut:restriction>
							<xsl:attribute name="base">
								<xsl:call-template name="GenerateXsdDataType"/>
							</xsl:attribute>
							<!-- check for length -->
							<xsOut:enumeration value="{$value}"/>
						</xsOut:restriction>
					</xsOut:simpleType>
				</xsl:for-each>
			</xsOut:union>
		</xsl:if>
		<xsl:if test="$rangeCount = 1 and $valueCount = 0">
			<xsOut:restriction>
				<xsl:attribute name="base">
					<xsl:call-template name="GenerateXsdDataType"/>
				</xsl:attribute>
				<!-- check for length -->
				<xsl:apply-templates select="oil:range"/>
			</xsOut:restriction>
		</xsl:if>
	</xsl:template>
	<!-- handels clusivity for range constraints -->
	<xsl:template match="odt:range">
		<xsl:variable name="lowerClusivity" select="odt:lowerBound/@clusivity"/>
		<xsl:variable name="upperClusivity" select="odt:upperBound/@clusivity"/>
		<xsl:variable name="lowerValue" select="odt:lowerBound/@value"/>
		<xsl:variable name="upperValue" select="odt:upperBound/@value"/>
		<xsl:if test="odt:lowerBound">
			<xsl:if test="$lowerClusivity = 'inclusive'">
				<xsOut:minInclusive value="{$lowerValue}"/>
			</xsl:if>
			<xsl:if test="$lowerClusivity = 'exclusive'">
				<xsOut:minExclusive value="{$lowerValue}"/>
			</xsl:if>
		</xsl:if>
		<xsl:if test="odt:upperBound">
			<xsl:if test="$upperClusivity = 'inclusive'">
				<xsOut:maxInclusive value="{$upperValue}"/>
			</xsl:if>
			<xsl:if test="$upperClusivity = 'exclusive'">
				<xsOut:maxExclusive value="{$upperValue}"/>
			</xsl:if>
		</xsl:if>
	</xsl:template>

	<xsl:template name="GenerateXsdDataType">
		<xsl:param name="Prefix" select="'xs'"/>
		<xsl:param name="ValueType" select="."/>
		<xsl:variable name="dataType" select="$ValueType/@dataType"/>
		<xsl:choose>
			<xsl:when test="$dataType='FixedLengthTextDataType'">
				<!--<xsl:if test="@length and @length!='0'">
							<xsOut:length value="{@length}"/>
						</xsl:if>-->
				<xsl:value-of select="concat($Prefix,':string')"/>
			</xsl:when>
			<xsl:when test="$dataType='VariableLengthTextDataType'">
				<xsl:value-of select="concat($Prefix,':string')"/>
			</xsl:when>
			<xsl:when test="$dataType='LargeLengthTextDataType'">
				<xsl:value-of select="concat($Prefix,':string')"/>
			</xsl:when>
			<xsl:when test="$dataType='SignedIntegerNumericDataType'">
				<xsl:value-of select="concat($Prefix,':integer')"/>
			</xsl:when>
			<xsl:when test="$dataType='AutoCounterNumericDataType'">
				<xsl:value-of select="concat($Prefix,':')"/>
			</xsl:when>
			<xsl:when test="$dataType='UnsignedIntegerNumericDataType'">
				<xsl:value-of select="concat($Prefix,':unsignedInt')"/>
			</xsl:when>
			<xsl:when test="$dataType='FloatingPointNumericDataType'">
				<xsl:value-of select="concat($Prefix,':float')"/>
			</xsl:when>
			<xsl:when test="$dataType='DecimalNumericDataType'">
				<xsl:value-of select="concat($Prefix,':decimal')"/>
			</xsl:when>
			<xsl:when test="$dataType='MoneyNumericDataType'">
				<xsl:value-of select="concat($Prefix,':decimal')"/>
			</xsl:when>
			<xsl:when test="$dataType='FixedLengthRawDataDataType'">
				<xsl:value-of select="concat($Prefix,':')"/>
			</xsl:when>
			<xsl:when test="$dataType='VariableLengthRawDataDataType'">
				<xsl:value-of select="concat($Prefix,':?')"/>
			</xsl:when>
			<xsl:when test="$dataType='LargeLengthRawDataDataType'">
				<xsl:value-of select="concat($Prefix,':?')"/>
			</xsl:when>
			<xsl:when test="$dataType='PictureRawDataDataType'">
				<xsl:value-of select="concat($Prefix,':?')"/>
			</xsl:when>
			<xsl:when test="$dataType='OleObjectRawDataDataType'">
				<xsl:value-of select="concat($Prefix,':?')"/>
			</xsl:when>
			<xsl:when test="$dataType='AutoTimestampTemporalDataType'">
				<xsl:value-of select="concat($Prefix,':?')"/>
			</xsl:when>
			<xsl:when test="$dataType='TimeTemporalDataType'">
				<xsl:value-of select="concat($Prefix,':time')"/>
			</xsl:when>
			<xsl:when test="$dataType='DateTemporalDataType'">
				<xsl:value-of select="concat($Prefix,':date')"/>
			</xsl:when>
			<xsl:when test="$dataType='DateAndTimeTemporalDataType'">
				<xsl:value-of select="concat($Prefix,':dateTime')"/>
			</xsl:when>
			<xsl:when test="$dataType='TrueOrFalseLogicalDataType'">
				<xsl:value-of select="concat($Prefix,':boolean')"/>
			</xsl:when>
			<xsl:when test="$dataType='YesOrNoLogicalDataType'">
				<xsl:value-of select="concat($Prefix,':boolean')"/>
			</xsl:when>
			<xsl:when test="$dataType='RowIdOtherDataType'">
				<xsl:value-of select="'{$Prefix}:'"/>
			</xsl:when>
			<xsl:when test="$dataType='ObjectIdOtherDataType'">
				<xsl:value-of select="'{$Prefix}:'"/>
			</xsl:when>
			<xsl:otherwise>
				<!--<xsl:message terminate="yes">Could not map DataType.</xsl:message>-->
			</xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	<xsl:template name="GenerateKeyConstraints">
		<xsl:for-each select="oil:conceptType">
			<xsOut:key name="{@name}Key">
				<xsOut:selector xpath="oxs:{@name}Elements/oxs:{@name}"/>
				<!-- complicated stuff to find the specified paths -->
				<xsl:variable name="fieldPath">

				</xsl:variable>
				<xsl:variable name="singleField" select="oil:informationType[oil:singleRoleUniquenessConstraint/@isPreferred='true']/@name"/>
				<xsl:if test="string-length($singleField)">
					<xsOut:field xpath="@{$singleField}"/>
				</xsl:if>
			</xsOut:key>
		</xsl:for-each>
	</xsl:template>
	<xsl:template name="GenerateUniquenessConstraints">
		<xsl:for-each select="oil:conceptType/oil:informationType/oil:singleRoleUniquenessConstraint">
			<xsl:variable name="MyName"  select="../@name"/>
			<xsl:variable name="MyParentsName" select="../../@name"/>
			<!--Only does one to one relationships-->
			<xsl:if test="@isPreferred!='true'">
				<xsOut:unique name="{$MyName}Unique">
					<xsOut:selector xpath="oxs:{/oil:model/@name}/oxs:{$MyParentsName}"/>
					<xsOut:field xpath="oxs:{$MyName}"/>
				</xsOut:unique>
			</xsl:if>
		</xsl:for-each>
	</xsl:template>
	<xsl:template name="GenerateKeyRefs">
		<xsl:for-each select="oil:conceptType">
			<xsl:for-each select="oil:conceptTypeRef">
				<xsl:variable name="parentName" select="../@name"/>
				<xsOut:keyref name="{$parentName}{@name}Ref" refer="{@target}Key">
					<xs:selector xpath="oxs:{$parentName}Elements/oxs:{$parentName}"/>
					<xsl:variable name="identifyer" select="../../oil:conceptType[@name=current()/@target]/oil:informationType[oil:singleRoleUniquenessConstraint/@isPreferred='true']/@name"/>
					<xs:field xpath="oxs:{@target}/@{$identifyer}"/>
				</xsOut:keyref>
			</xsl:for-each>
		</xsl:for-each>
	</xsl:template>
</xsl:stylesheet>
